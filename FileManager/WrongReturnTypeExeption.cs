namespace FileManager
{
    /// <summary>
    /// This exeption is raised if a delegate's return type is not what is should be.
    /// </summary>
    public class WrongReturnTypeExeption : Exception
    {
        #region Constructors
        /// <summary>
        /// <inheritdoc cref="WrongReturnTypeExeption"/>
        /// </summary>
        public WrongReturnTypeExeption()
            : this("Delegate's return type in wrong.") { }

        /// <summary>
        /// <inheritdoc cref="WrongReturnTypeExeption"/>
        /// </summary>
        /// <param name="message">The message to display.</param>
        public WrongReturnTypeExeption(string message) : base(message) { }
        #endregion
    }
}
