using System.Linq;
using Xunit;

namespace WixSharp.Test
{
    public class ManagedActionsTest
    {
        [Fact]
        public void Should_Normalize_AsmReferences()
        {
            var project = new Project("test",
                new ElevatedManagedAction("action_0"),
                new ManagedAction("action_1"),
                new ManagedAction("action_2") { RefAssemblies = new[] { "asm1.dll", "asm2.dll" } },
                new ManagedAction("action_3") { RefAssemblies = new[] { "asm2.dll", "asm3.dll" } },
                new ManagedAction("action_11") { ActionAssembly = "test.dll" },
                new ManagedAction("action_22") { ActionAssembly = "test.dll", RefAssemblies = new[] { "asm5.dll", "asm7.dll" } },
                new ManagedAction("action_33") { ActionAssembly = "test.dll", RefAssemblies = new[] { "asm6.dll", "asm8.dll" } });

            project.Preprocess();

            var actions = project.Actions.OfType<ManagedAction>().ToArray();
            var action = actions[0];
            Assert.Equal("%this%", action.ActionAssembly);
            Assert.Equal(3, action.RefAssemblies.Length);
            Assert.Contains("asm1.dll", action.RefAssemblies);
            Assert.Contains("asm2.dll", action.RefAssemblies);
            Assert.Contains("asm3.dll", action.RefAssemblies);

            action = actions[1];
            Assert.Equal("%this%", action.ActionAssembly);
            Assert.Equal(3, action.RefAssemblies.Length);
            Assert.Contains("asm1.dll", action.RefAssemblies);
            Assert.Contains("asm2.dll", action.RefAssemblies);
            Assert.Contains("asm3.dll", action.RefAssemblies);

            action = actions[2];
            Assert.Equal("%this%", action.ActionAssembly);
            Assert.Equal(3, action.RefAssemblies.Length);
            Assert.Contains("asm1.dll", action.RefAssemblies);
            Assert.Contains("asm2.dll", action.RefAssemblies);
            Assert.Contains("asm3.dll", action.RefAssemblies);

            action = actions[3];
            Assert.Equal("%this%", action.ActionAssembly);
            Assert.Equal(3, action.RefAssemblies.Length);
            Assert.Contains("asm1.dll", action.RefAssemblies);
            Assert.Contains("asm2.dll", action.RefAssemblies);
            Assert.Contains("asm3.dll", action.RefAssemblies);

            action = actions[4];
            Assert.Equal("test.dll", action.ActionAssembly);
            Assert.Equal(4, action.RefAssemblies.Length);
            Assert.Contains("asm5.dll", action.RefAssemblies);
            Assert.Contains("asm6.dll", action.RefAssemblies);
            Assert.Contains("asm7.dll", action.RefAssemblies);
            Assert.Contains("asm8.dll", action.RefAssemblies);

            action = actions[5];
            Assert.Equal("test.dll", action.ActionAssembly);
            Assert.Equal(4, action.RefAssemblies.Length);
            Assert.Contains("asm5.dll", action.RefAssemblies);
            Assert.Contains("asm6.dll", action.RefAssemblies);
            Assert.Contains("asm7.dll", action.RefAssemblies);
            Assert.Contains("asm8.dll", action.RefAssemblies);

            action = actions[6];
            Assert.Equal("test.dll", action.ActionAssembly);
            Assert.Equal(4, action.RefAssemblies.Length);
            Assert.Contains("asm5.dll", action.RefAssemblies);
            Assert.Contains("asm6.dll", action.RefAssemblies);
            Assert.Contains("asm7.dll", action.RefAssemblies);
            Assert.Contains("asm8.dll", action.RefAssemblies);
        }
    }
}