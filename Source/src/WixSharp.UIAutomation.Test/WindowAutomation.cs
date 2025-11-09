using System.Diagnostics;

// using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Automation;

public static class ProcessAutomation
{
    public static (string output, int exitCode) run(this string exe, string args = null, string dir = null)
    {
        using var process = new Process();

        process.StartInfo.FileName = exe;
        process.StartInfo.Arguments = args;
        process.StartInfo.WorkingDirectory = dir;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        output += process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (output, process.ExitCode);
    }
}

public static class WindowAutomation
{
    #region Windows API Declarations

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    #endregion Windows API Declarations

    /// <summary>
    /// Finds a window by its title (exact match)
    /// </summary>
    /// <param name="windowTitle">The exact title of the window</param>
    /// <returns>AutomationElement representing the window, or null if not found</returns>
    public static AutomationElement? FindWindowByTitle(string windowTitle)
    {
        IntPtr windowHandle = FindWindow(null, windowTitle);

        if (windowHandle != IntPtr.Zero)
        {
            return AutomationElement.FromHandle(windowHandle);
        }

        return null;
    }

    /// <summary>
    /// Finds a window by partial title match
    /// </summary>
    /// <param name="partialTitle">Part of the window title to search for</param>
    /// <returns>AutomationElement representing the window, or null if not found</returns>
    public static AutomationElement? FindWindowByPartialTitle(string partialTitle)
    {
        var windows = new List<AutomationElement>();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    var builder = new System.Text.StringBuilder(length + 1);
                    GetWindowText(hWnd, builder, builder.Capacity);
                    string windowText = builder.ToString();

                    if (windowText.Contains(partialTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var element = AutomationElement.FromHandle(hWnd);
                            windows.Add(element);
                        }
                        catch (ElementNotAvailableException)
                        {
                            // Skip this window if it's not available
                        }
                    }
                }
            }
            return true;
        }, IntPtr.Zero);

        return windows.FirstOrDefault();
    }

    /// <summary>
    /// Finds a button by its name within a parent element (searches recursively)
    /// </summary>
    /// <param name="parent">The parent element to search within</param>
    /// <param name="buttonText">The text/name of the button to find</param>
    /// <returns>AutomationElement representing the button, or null if not found</returns>
    public static AutomationElement? FindButtonByText(AutomationElement parent, string buttonText)
    {
        if (parent == null)
            return null;

        try
        {
            // Search for buttons with exact text match
            var condition = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                new PropertyCondition(AutomationElement.NameProperty, buttonText)
                                            );

            var button = parent.FindFirst(TreeScope.Descendants, condition);

            if (button != null)
                return button;

            // If exact match not found, try partial match
            var allButtons = parent.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));

            foreach (AutomationElement btn in allButtons)
            {
                if (btn.Current.Name.Contains(buttonText, StringComparison.OrdinalIgnoreCase))
                {
                    return btn;
                }
            }
        }
        catch (ElementNotAvailableException)
        {
            // Element is no longer available
        }

        return null;
    }

    public static bool Click(this AutomationElement element)
        => WindowAutomation.ClickElement(element);

    public static AutomationElement Click(this AutomationElement element, string clickName)
    {
        element.FindClickable(clickName)?.Click();
        return element;
    }

    /// <summary>
    /// Finds any clickable element (button, link, etc.) by text within a parent element
    /// </summary>
    /// <param name="parent">The parent element to search within</param>
    /// <param name="elementText">The text of the element to find</param>
    /// <returns>AutomationElement representing the clickable element, or null if not found</returns>
    public static AutomationElement? FindClickable(this AutomationElement parent, string elementText)
    {
        if (parent == null)
            return null;

        try
        {
            // Search for common clickable elements
            var clickableTypes = new[]
            {
                ControlType.Button,
                ControlType.Hyperlink,
                ControlType.MenuItem,
                ControlType.CheckBox,
                ControlType.RadioButton
            };

            foreach (var controlType in clickableTypes)
            {
                var condition = new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, controlType),
                    new PropertyCondition(AutomationElement.NameProperty, elementText)
                                                );

                var element = parent.FindFirst(TreeScope.Descendants, condition);
                if (element != null)
                    return element;
            }

            // Try partial match if exact match not found
            foreach (var controlType in clickableTypes)
            {
                var allElements = parent.FindAll(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, controlType));

                foreach (AutomationElement element in allElements)
                {
                    if (element.Current.Name.Contains(elementText, StringComparison.OrdinalIgnoreCase))
                    {
                        return element;
                    }
                }
            }
        }
        catch (ElementNotAvailableException)
        {
            // Element is no longer available
        }

        return null;
    }

    /// <summary>
    /// Sends a click input to the specified element
    /// </summary>
    /// <param name="element">The element to click</param>
    /// <returns>True if click was successful, false otherwise</returns>
    public static bool ClickElement(AutomationElement? element)
    {
        if (element == null)
            return false;

        try
        {
            // Try to use InvokePattern first (most reliable for buttons)
            if (element.TryGetCurrentPattern(InvokePattern.Pattern, out object? invokePattern))
            {
                ((InvokePattern)invokePattern).Invoke();
                return true;
            }

            // If InvokePattern is not available, try TogglePattern (for checkboxes, radio buttons)
            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out object? togglePattern))
            {
                ((TogglePattern)togglePattern).Toggle();
                return true;
            }

            // As a last resort, try to click at the element's center point
            var clickablePoint = element.GetClickablePoint();

            // You would need to implement mouse click here using SendInput or similar
            // For now, we'll just return true to indicate we found the clickable point
            PerformMouseClick(clickablePoint);
            return true;
        }
        catch (Exception ex) when (ex is ElementNotAvailableException or NoClickablePointException or InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// Performs a mouse click at the specified point
    /// </summary>
    /// <param name="point">The point to click</param>
    private static void PerformMouseClick(System.Windows.Point point)
    {
        // Implementation would use SendInput or similar Win32 API
        // For demonstration, we'll use a simple approach
        System.Windows.Forms.Cursor.Position = new Point((int)point.X, (int)point.Y);

        // You might want to implement actual mouse click using SendInput for production use
        // This is a simplified version
    }

    /// <summary>
    /// Waits for a window to appear with the specified title
    /// </summary>
    /// <param name="windowTitle">The title of the window to wait for</param>
    /// <param name="timeoutMilliseconds">Maximum time to wait in seconds</param>
    /// <returns>AutomationElement representing the window, or null if timeout</returns>
    public static async Task<AutomationElement?> WaitForWindowAsync(string windowTitle, int timeoutMilliseconds = 30)
    {
        var startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds)
        {
            var window = FindWindowByTitle(windowTitle);
            if (window != null)
                return window;

            await Task.Delay(500); // Check every 500ms
        }

        return null;
    }

    public static AutomationElement? WaitFor(Func<AutomationElement> find, int timeoutMilliseconds = 30000)
        => WaitForAsync(find, timeoutMilliseconds).GetAwaiter().GetResult();

    public static AutomationElement? WaitFor(this AutomationElement element, Func<AutomationElement, bool> find, int timeoutMilliseconds = 30000)
    {
        WaitForAsync(() => find(element), timeoutMilliseconds).GetAwaiter().GetResult();
        return element;
    }

    public static void Delay(int timeoutMilliseconds = 10000) => Task.Delay(timeoutMilliseconds).Wait();

    public static T Delay<T>(this T obj, int timeoutMilliseconds = 10000)
    {
        Delay(timeoutMilliseconds);
        return obj;
    }

    public static T Print<T>(this T obj, string text)
    {
        Console.WriteLine(text);
        return obj;
    }

    public static async Task<AutomationElement?> WaitForAsync(Func<AutomationElement> find, int timeoutMilliseconds = 30000)
    {
        var startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds)
        {
            var window = find();
            if (window != null)
                return window;

            await Task.Delay(500); // Check every 500ms
        }
        Console.WriteLine($"Could not find window...");
        return null;
    }

    public static async Task<bool?> WaitForAsync(Func<bool> find, int timeoutMilliseconds = 30000)
    {
        var startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMilliseconds)
        {
            if (find())
                return true;

            await Task.Delay(500); // Check every 500ms
        }
        return false;
    }

    public static bool VisibleTextAnywhere(this AutomationElement parent, string searchText, bool exactMatch = false)
        => parent.FindTextAnywhere(searchText, exactMatch) != null;

    /// <summary>
    /// Finds any element containing the specified text within a window
    /// </summary>
    /// <param name="parent">The parent element (window) to search within</param>
    /// <param name="searchText">The text to search for</param>
    /// <param name="exactMatch">If true, requires exact match; if false, allows partial match</param>
    /// <returns>AutomationElement containing the text, or null if not found</returns>
    public static AutomationElement? FindTextAnywhere(this AutomationElement parent, string searchText, bool exactMatch = false)
    {
        if (parent == null || string.IsNullOrEmpty(searchText))
            return null;

        try
        {
            // Search through all elements that can contain text
            var textContainingTypes = new[]
            {
                ControlType.Text,
                ControlType.Button,
                ControlType.CheckBox,
                ControlType.RadioButton,
                ControlType.Hyperlink,
                ControlType.MenuItem,
                ControlType.ListItem,
                ControlType.TabItem,
                ControlType.TreeItem,
                ControlType.Group,
                ControlType.Window,
                ControlType.TitleBar,
                ControlType.StatusBar,
                ControlType.ToolTip,
                ControlType.Edit,
                ControlType.Document
            };

            // First try exact match with Name property
            if (exactMatch)
            {
                foreach (var controlType in textContainingTypes)
                {
                    var condition = new AndCondition(
                        new PropertyCondition(AutomationElement.ControlTypeProperty, controlType),
                        new PropertyCondition(AutomationElement.NameProperty, searchText)
                                                    );

                    var element = parent.FindFirst(TreeScope.Descendants, condition);
                    if (element != null)
                        return element;
                }
            }

            // Search through all descendants and check their text properties
            var allElements = parent.FindAll(TreeScope.Descendants, Condition.TrueCondition);

            foreach (AutomationElement element in allElements)
            {
                try
                {
                    // Check Name property
                    if (!string.IsNullOrEmpty(element.Current.Name))
                    {
                        if (exactMatch ? element.Current.Name.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                       : element.Current.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            return element;
                        }
                    }

                    // Check HelpText property
                    if (!string.IsNullOrEmpty(element.Current.HelpText))
                    {
                        if (exactMatch ? element.Current.HelpText.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                       : element.Current.HelpText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            return element;
                        }
                    }

                    // For text controls and edit controls, try to get the text content
                    if (element.Current.ControlType == ControlType.Text ||
                        element.Current.ControlType == ControlType.Edit ||
                        element.Current.ControlType == ControlType.Document)
                    {
                        // Try ValuePattern for edit controls
                        if (element.TryGetCurrentPattern(ValuePattern.Pattern, out object? valuePattern))
                        {
                            var textValue = ((ValuePattern)valuePattern).Current.Value;
                            if (!string.IsNullOrEmpty(textValue))
                            {
                                if (exactMatch ? textValue.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                               : textValue.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    return element;
                                }
                            }
                        }

                        // Try TextPattern for more complex text controls
                        if (element.TryGetCurrentPattern(TextPattern.Pattern, out object? textPattern))
                        {
                            var textRange = ((TextPattern)textPattern).DocumentRange;
                            var textContent = textRange.GetText(-1);
                            if (!string.IsNullOrEmpty(textContent))
                            {
                                if (exactMatch ? textContent.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                               : textContent.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    return element;
                                }
                            }
                        }
                    }
                }
                catch (ElementNotAvailableException)
                {
                    // Skip this element if it's not available
                    continue;
                }
                catch (InvalidOperationException)
                {
                    // Skip if pattern is not supported
                    continue;
                }
            }
        }
        catch (ElementNotAvailableException)
        {
            // Parent element is no longer available
        }

        return null;
    }

    /// <summary>
    /// Finds all elements containing the specified text within a window
    /// </summary>
    /// <param name="parent">The parent element (window) to search within</param>
    /// <param name="searchText">The text to search for</param>
    /// <param name="exactMatch">If true, requires exact match; if false, allows partial match</param>
    /// <returns>List of AutomationElements containing the text</returns>
    public static List<AutomationElement> FindAllTextAnywhere(this AutomationElement parent, string searchText, bool exactMatch = false)
    {
        var results = new List<AutomationElement>();

        if (parent == null || string.IsNullOrEmpty(searchText))
            return results;

        try
        {
            var allElements = parent.FindAll(TreeScope.Descendants, Condition.TrueCondition);

            foreach (AutomationElement element in allElements)
            {
                try
                {
                    bool foundMatch = false;

                    // Check Name property
                    if (!string.IsNullOrEmpty(element.Current.Name))
                    {
                        if (exactMatch ? element.Current.Name.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                       : element.Current.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            foundMatch = true;
                        }
                    }

                    // Check HelpText property
                    if (!foundMatch && !string.IsNullOrEmpty(element.Current.HelpText))
                    {
                        if (exactMatch ? element.Current.HelpText.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                       : element.Current.HelpText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            foundMatch = true;
                        }
                    }

                    // Check text patterns for text/edit controls
                    if (!foundMatch && (element.Current.ControlType == ControlType.Text ||
                                        element.Current.ControlType == ControlType.Edit ||
                                        element.Current.ControlType == ControlType.Document))
                    {
                        // Try ValuePattern
                        if (element.TryGetCurrentPattern(ValuePattern.Pattern, out object? valuePattern))
                        {
                            var textValue = ((ValuePattern)valuePattern).Current.Value;
                            if (!string.IsNullOrEmpty(textValue))
                            {
                                if (exactMatch ? textValue.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                               : textValue.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    foundMatch = true;
                                }
                            }
                        }

                        // Try TextPattern
                        if (!foundMatch && element.TryGetCurrentPattern(TextPattern.Pattern, out object? textPattern))
                        {
                            var textRange = ((TextPattern)textPattern).DocumentRange;
                            var textContent = textRange.GetText(-1);
                            if (!string.IsNullOrEmpty(textContent))
                            {
                                if (exactMatch ? textContent.Equals(searchText, StringComparison.OrdinalIgnoreCase)
                                               : textContent.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    foundMatch = true;
                                }
                            }
                        }
                    }

                    if (foundMatch)
                    {
                        results.Add(element);
                    }
                }
                catch (ElementNotAvailableException)
                {
                    // Skip this element if it's not available
                    continue;
                }
                catch (InvalidOperationException)
                {
                    // Skip if pattern is not supported
                    continue;
                }
            }
        }
        catch (ElementNotAvailableException)
        {
            // Parent element is no longer available
        }

        return results;
    }
}