namespace FileManager
{
    /// <summary>
    /// This exeption is raised if "fileName" and "seed" are both null or "fileName" doesn't contain the special character ("*") in <c>ReadFiles</c>.
    /// </summary>
    public class ReadFilesArgsExeption : Exception
    {
        #region Constructors
        /// <summary>
        /// <inheritdoc cref="ReadFilesArgsExeption" path="//summary"/>
        /// </summary>
        public ReadFilesArgsExeption()
            : this($"\"fileName\" and \"seed\" can\'t both be null at the same time, and \"fileName\" must contain at least one \"{Utils.FILE_NAME_SEED_REPLACE_STRING}\"") { }

        /// <summary>
        /// <inheritdoc cref="ReadFilesArgsExeption" path="//summary"/>
        /// </summary>
        /// <param name="message">The message to display.</param>
        public ReadFilesArgsExeption(string message) : base(message) { }
        #endregion
    }
}
