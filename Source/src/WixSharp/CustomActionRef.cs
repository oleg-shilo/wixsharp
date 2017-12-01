namespace WixSharp
{
    /// <summary>
    /// This will cause the entire contents of the Fragment containing the referenced CustomAction to be included in the installer database.
    /// </summary>
    public class CustomActionRef : Action
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionRef" /> class.
        /// </summary>
        /// <param name="when">The When.</param>
        /// <param name="step">The Step.</param>
        public CustomActionRef(When when, Step step)
        {
            When = when;
            Step = step;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionRef" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="when">The When.</param>
        /// <param name="step">The Step.</param>
        public CustomActionRef(string id, When when, Step step)
            : base(new Id(id))
        {
            When = when;
            Step = step;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionRef" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="when">The When.</param>
        /// <param name="step">The Step.</param>
        public CustomActionRef(Id id, When when, Step step)
            : base(id)
        {
            When = when;
            Step = step;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionRef" /> class.
        /// </summary>
        /// <param name="when">The When.</param>
        /// <param name="step">The Step.</param>
        /// <param name="condition">The Condition.</param>
        public CustomActionRef(When when, Step step, Condition condition)
        {
            When = when;
            Step = step;
            Condition = condition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionRef" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="when">The When.</param>
        /// <param name="step">The Step.</param>
        /// <param name="condition">The Condition.</param>
        public CustomActionRef(string id, When when, Step step, Condition condition)
            : base(new Id(id))
        {
            When = when;
            Step = step;
            Condition = condition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionRef" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="when">The When.</param>
        /// <param name="step">The Step.</param>
        /// <param name="condition">The Condition.</param>
        public CustomActionRef(Id id, When when, Step step, Condition condition)
            : base(id)
        {
            When = when;
            Step = step;
            Condition = condition;
        }
    }
}