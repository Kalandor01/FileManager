namespace FileManager
{
    /// <summary>
    /// Posible responses from file managers in <c>FileManager</c>.
    /// </summary>
    public enum FileManagerOptions : int
    {
        /// <summary>
        /// Returned by a file manager method, when the user exits.
        /// </summary>
        EXIT = -1,
        /// <summary>
        /// Returned by a file manager method, when the user creates a new file.
        /// </summary>
        NEW_FILE = 0,
        /// <summary>
        /// Returned by a file manager method, when the user loads a file.
        /// </summary>
        LOAD_FILE = 1,
        /// <summary>
        /// Returned by a file manager method, when the user deletes a file.
        /// </summary>
        DELETE_FILE = 2
    }
}
