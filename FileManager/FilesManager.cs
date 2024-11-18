namespace FileManager
{
    /// <summary>
    /// Contains functions for managing (creating, loading, deleting) files.<br/>
    /// ONLY USABLE WITH <c>ConsoleUI</c> INSTALLED!
    /// </summary>
    public static class FilesManager
    {
        #region Public functions
        /// <summary>
        /// Allows the user to pick between creating a new save, loading an old save and deleteing a save.<br/>
        /// Reads in file data from a <c>FileReader</c> method.<br/>
        /// Returns a tuple depending on what the user selected, containig what happenend, and in whitch slot.<br/>
        /// Can return: new, load, delete
        /// </summary>
        /// <param name="filesData">The return value from a <c>FileReader</c> method.</param>
        /// <param name="maxFiles">The maximum number of files that can exist. If the number of files go abowe this number, no new files can be created. -1 for no limit.</param>
        /// <param name="fileName">The name of the files without the extension. The a "*"-s in the name will be replaced with the file number.</param>
        /// <param name="fileExt">The extension of the files.</param>
        /// <returns></returns>
        public static (FileManagerOptions ManagerOption, int slotNumber) ManageFiles(Dictionary<string, List<string>?> filesData, int maxFiles = -1, string fileName = "file*", string fileExt = "savc")
        {
            var option = 1;
            var manageExit = false;
            while (!manageExit)
            {
                if (filesData.Count > 0)
                {
                    // get file range
                    var maxFileNum = 0;
                    var minFileNum = int.MaxValue;
                    foreach (var fileNumStr in filesData.Keys)
                    {
                        if (int.TryParse(fileNumStr, out int fileNum))
                        {
                            if (fileNum > maxFileNum)
                            {
                                maxFileNum = fileNum;
                            }
                            if (fileNum < minFileNum)
                            {
                                minFileNum = fileNum;
                            }
                        }
                    }
                    option = Utils.ReadInt($"Select an option: -1: delete mode, 0: new file, {minFileNum}-{maxFileNum}: load file: ");
                    // delete
                    if (option == -1)
                    {
                        option = Utils.ReadInt($"Select an option: 0: back, {minFileNum}-{maxFileNum}: delete file: ");
                        if (filesData.ContainsKey(option.ToString()))
                        {
                            Console.Write($"Are you sure you want to remove Save file {option}?(Y/N): ");
                            var sure = Console.ReadLine();
                            if (sure is not null && sure.ToUpper() == "Y")
                            {
                                File.Delete($"{fileName.Replace(Utils.FILE_NAME_SEED_REPLACE_STRING, option.ToString())}.{fileExt}");
                                manageExit = true;
                            }
                        }
                        else if (option != 0)
                        {
                            Console.WriteLine($"Save file {option} doesn't exist!");
                        }
                    }
                    // new file
                    else if (option == 0)
                    {
                        var newSlot = 1;
                        foreach (var fileNumStr in filesData.Keys)
                        {
                            if (int.TryParse(fileNumStr, out int fileNum) && fileNum == newSlot)
                            {
                                newSlot++;
                            }
                        }
                        if (maxFiles < 0 || newSlot <= maxFiles)
                        {
                            return (FileManagerOptions.NEW_FILE, newSlot);
                        }
                        else
                        {
                            Utils.PressKey("No empty save files! Delete a file to continue!");
                        }
                    }
                    // load
                    else
                    {
                        if (filesData.ContainsKey(option.ToString()))
                        {
                            return (FileManagerOptions.LOAD_FILE, option);
                        }
                        Console.WriteLine($"Save file {option} doesn't exist!");
                    }
                }
                else
                {
                    Utils.PressKey("No save files!");
                    return (FileManagerOptions.NEW_FILE, 1);
                }
            }
            return (FileManagerOptions.DELETE_FILE, option);
        }

        /*/// <summary>
        /// Allows the user to pick between creating a new save, loading an old save and deleteing a save, with UI selection.<br/>
        /// Reads in a file data dictionary, where the first element is the file number/seed, and the second is the text to display for that file.<br/>
        /// Returns a tuple depending on what the user selected, containig what happenend, and in whitch slot (except exit).<br/>
        /// Can return: new, load, exit
        /// </summary>
        /// <param name="filesDataProcessed">A file data dictionary, where the first element is the file number/seed, and the second is the text to display for that file.</param>
        /// <param name="maxFiles">The maximum number of files that can exist. If the number of files go abowe this number, no new files can be created. -1 for no limit.</param>
        /// <param name="fileName">The name of the files without the extension. The a "*"-s in the name will be replaced with the file number.</param>
        /// <param name="fileExt">The extension of the files.</param>
        /// <param name="canExit">If the user can exit from this menu with he key assigned to the escape action.</param>
        /// <param name="keybinds">The list of <c>KeyAction</c> objects to use.</param>
        /// <param name="keyResults">The list of posible results returned by pressing a key.</param>
        /// <returns></returns>*/
        //public static (FileManagerOptions ManagerOption, int slotNumber) ManageFilesUI(
        //    Dictionary<int, string> filesDataProcessed,
        //    int maxFiles = -1,
        //    string fileName = "file*",
        //    string fileExt = "savc",
        //    bool canExit = false,
        //    IEnumerable<KeyAction>? keybinds = null,
        //    IEnumerable<object>? keyResults = null
        //)
        //{
        //    var inMainMenu = true;
        //    int option;
        //    while (true)
        //    {
        //        if (filesDataProcessed.Count > 0)
        //        {
        //            if (inMainMenu)
        //            {
        //                inMainMenu = false;
        //                option = (int)new UIList(new List<string?> { "New save", "Load/Delete save" }, " Main menu", canEscape:canExit).Display(keybinds, keyResults);
        //            }
        //            else
        //            {
        //                option = 1;
        //            }
        //            // new file
        //            if (option == 0)
        //            {
        //                var newSlot = 1;
        //                foreach (var fileNum in filesDataProcessed.Keys)
        //                {
        //                    if (fileNum == newSlot)
        //                    {
        //                        newSlot++;
        //                    }
        //                }
        //                if (maxFiles < 0 || newSlot <= maxFiles)
        //                {
        //                    return (FileManagerOptions.NEW_FILE, newSlot);
        //                }
        //                else
        //                {
        //                    Utils.PressKey("No empty save files! Delete a file to continue!");
        //                }
        //            }
        //            // exit
        //            else if (option == -1)
        //            {
        //                return (FileManagerOptions.EXIT, -1);
        //            }
        //            // load / delete
        //            else
        //            {
        //                // get data from file_data
        //                var listData = new List<string?>();
        //                foreach (var data in filesDataProcessed)
        //                {
        //                    listData.Add(data.Value);
        //                }
        //                listData.Add(null);
        //                listData.Add("Delete file");
        //                listData.Add("Back");
        //                option = (int)new UIList(listData, " Level select", canEscape: true).Display(keybinds, keyResults);
        //                // load
        //                if (option != -1 && option < filesDataProcessed.Count)
        //                {
        //                    return (FileManagerOptions.LOAD_FILE, filesDataProcessed.ElementAt(option).Key);
        //                }
        //                // delete
        //                else if (option == filesDataProcessed.Count + 1)
        //                {
        //                    listData.RemoveAt(listData.Count - 2);
        //                    var deleteMode = true;
        //                    while (deleteMode && filesDataProcessed.Count > 0)
        //                    {
        //                        option = (int)new UIList(listData, " Delete mode!", new CursorIcon("X ", "", "  "), false, true).Display(keybinds, keyResults);
        //                        if (option != -1 && option != listData.Count - 1)
        //                        {
        //                            var sure = (int)new UIList(new List<string?> { "No", "Yes" }, $" Are you sure you want to remove Save file {filesDataProcessed.ElementAt(option).Key}?", canEscape: true).Display(keybinds, keyResults);
        //                            if (sure == 1)
        //                            {
        //                                File.Delete($"{fileName.Replace(Utils.FILE_NAME_SEED_REPLACE_STRING, filesDataProcessed.ElementAt(option).Key.ToString())}.{fileExt}");
        //                                listData.RemoveAt(option);
        //                                filesDataProcessed.Remove(filesDataProcessed.ElementAt(option).Key);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            deleteMode = false;
        //                        }
        //                    }
        //                }
        //                // back
        //                else
        //                {
        //                    inMainMenu = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Utils.PressKey("\n No save files detected!");
        //            return (FileManagerOptions.NEW_FILE, 1);
        //        }
        //    }
        //}

        /*/// <summary>
        /// Allows the user to pick between creating a new save, loading an old save and deleteing a save, with UI selection.<br/>
        /// The <c>newFileFunction</c> and the <c>loadFileFunction</c> delegates run, when the user creates or loads a save file, and both WILL get the file number, that was refrenced as their first argument.<br/>
        /// The <c>GetDataFunction</c> MUST return an IDictionary where the key is the number/seed of the file, and the value is the text to display for that file.
        /// </summary>
        /// <param name="newFileFunction">The function and its arguments, called when the user creates a new file.</param>
        /// <param name="loadFileFunction">The function and its arguments, called when the user loads a file.</param>
        /// <param name="getDataFunction">The function and its arguments, used for getting all files in a folder.</param>
        /// <param name="maxFiles">The maximum number of files that can exist. If the number of files go abowe this number, no new files can be created. -1 for no limit.</param>
        /// <param name="fileName">The name of the files without the extension. The a "*"-s in the name will be replaced with the file number.</param>
        /// <param name="fileExt">The extension of the files.</param>
        /// <param name="canExit">If the user can exit from this menu with he key assigned to the escape action.</param>
        /// <param name="keybinds">The list of <c>KeyAction</c> objects to use.</param>
        /// <param name="keyResults">The list of posible results returned by pressing a key.</param>*/
        //public static void ManageFilesUIAdvanced(
        //    (Delegate function, object?[]? args) newFileFunction,
        //    (Delegate function, object?[]? args) loadFileFunction,
        //    (Delegate function, object?[]? args) getDataFunction,
        //    int maxFiles = -1,
        //    string fileName = "file*",
        //    string fileExt = "savc",
        //    bool canExit = false,
        //    IEnumerable<KeyAction>? keybinds = null,
        //    IEnumerable<object>? keyResults = null
        //)
        //{
        //    var getDataFunctionReturn = getDataFunction.function.Method.ReturnType;
        //    if (
        //        !(getDataFunctionReturn is not null &&
        //        typeof(IDictionary<int, string>).IsAssignableFrom(getDataFunctionReturn))
        //    )
        //    {
        //        throw new WrongReturnTypeExeption("The get data delegate's return type should be of type IDictionary<int, string>.");
        //    }

        //    while (true)
        //    {
        //        var filesData = (IDictionary<int, string>?)getDataFunction.function.DynamicInvoke(getDataFunction.args);
        //        // main
        //        if (filesData is not null && filesData.Count > 0)
        //        {
        //            var option = (int)new UIList(new List<string> { "New save", "Load/Delete save" }, " Main menu", canEscape: canExit, actions: new List<UIAction> {
        //                new UIAction(new NewFunctionDelegate(NewFilePreperation), getDataFunction, newFileFunction, maxFiles),
        //                new UIAction(new LoadDeleteFunctionDelegate(LoadOrDeleteMenu), getDataFunction, newFileFunction, loadFileFunction, keybinds, keyResults, fileName, fileExt)
        //            }).Display(keybinds, keyResults);
        //            if (option == -1)
        //            {
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            Utils.PressKey("\n No save files detected!");
        //            InvokeActionWithFileNum(newFileFunction, 1);
        //        }
        //    }
        //}
        #endregion

        #region Private functions
        /// <summary>
        /// Method invoked when the user clicks on the new save button.
        /// </summary>
        /// <inheritdoc cref="ManageFilesUIAdvanced"/>
        private static void NewFilePreperation(
            (Delegate function, object?[]? args) getDataFunction,
            (Delegate function, object?[]? args) newFileFunction,
            int maxFiles
        )
        {
            var filesData = (IDictionary<int, string>?)getDataFunction.function.DynamicInvoke(getDataFunction.args);
            var newSlot = 1;
            foreach (var data in filesData)
            {
                if (data.Key == newSlot)
                {
                    newSlot++;
                }
            }
            if (maxFiles < 0 || newSlot <= maxFiles)
            {
                InvokeActionWithFileNum(newFileFunction, newSlot);
            }
            else
            {
                Utils.PressKey("No empty save files! Delete a file to continue!");
            }
        }

        /*/// <summary>
        /// Method invoked when the user clicks on the load/delete button.
        /// </summary>
        /// <inheritdoc cref="ManageFilesUIAdvanced"/>*/
        //private static void LoadOrDeleteMenu(
        //    (Delegate function, object?[]? args) getDataFunction,
        //    (Delegate function, object?[]? args) newFileFunction,
        //    (Delegate function, object?[]? args) loadFileFunction,
        //    IEnumerable<KeyAction>? keybinds,
        //    IEnumerable<object>? keyResults,
        //    string fileName,
        //    string fileExt
        //)
        //{
        //    while (true)
        //    {
        //        // get data from filesData
        //        var filesData = (IDictionary<int, string>?)getDataFunction.function.DynamicInvoke(getDataFunction.args);
        //        var listData = new List<string?>();
        //        foreach (var data in filesData.Values)
        //        {
        //            listData.Add(data);
        //            listData.Add(null);
        //        }
        //        listData.Add("Delete file");
        //        listData.Add("Back");
        //        var option = (int)new UIList(listData, " Level select", canEscape: true).Display(keybinds, keyResults);
        //        // load
        //        if (option != -1 && option / 2 < filesData.Count)
        //        {
        //            InvokeActionWithFileNum(loadFileFunction, filesData.ElementAt(option / 2).Key);
        //        }
        //        // delete
        //        else if (option == filesData.Count * 2)
        //        {
        //            listData.RemoveAt(listData.Count - 2);
        //            var deleteMode = true;
        //            while (deleteMode && filesData.Count > 0)
        //            {
        //                option = (int)new UIList(listData, " Delete mode!", new CursorIcon("X ", "", "  "), false, true).Display(keybinds, keyResults);
        //                if (option != -1 && option != listData.Count - 1)
        //                {
        //                    option /= 2;
        //                    var sure = (int)new UIList(new List<string?> { "No", "Yes" }, $" Are you sure you want to remove Save file {filesData.ElementAt(option).Key}?", canEscape: true).Display(keybinds, keyResults);
        //                    if (sure == 1)
        //                    {
        //                        File.Delete($"{fileName.Replace(Utils.FILE_NAME_SEED_REPLACE_STRING, filesData.ElementAt(option).Key.ToString())}.{fileExt}");
        //                        listData.RemoveAt(option * 2);
        //                        listData.RemoveAt(option * 2);
        //                        filesData.Remove(filesData.ElementAt(option));
        //                    }
        //                }
        //                else
        //                {
        //                    deleteMode = false;
        //                }
        //            }
        //            if (filesData.Count == 0)
        //            {
        //                Utils.PressKey("\n No save files detected!");
        //                InvokeActionWithFileNum(newFileFunction, 1);
        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}

        /// <summary>
        /// Helper function, used to invoke a delegate, with its arguments, with an added file number as its first argument.
        /// </summary>
        /// <param name="action">The delegate and its arguments.</param>
        /// <param name="fileNum">The file number.</param>
        private static void InvokeActionWithFileNum((Delegate function, object?[]? args) action, int fileNum)
        {
            var functionArgs = new List<object?> { fileNum };
            if (action.args is not null)
            {
                functionArgs.AddRange(action.args);
            }
            action.function.DynamicInvoke([.. functionArgs]);
        }
        #endregion

        #region Private delegates
        /// <summary>
        /// Helper delegate for when the user clicks the new save buitton.
        /// </summary>
        /// <inheritdoc cref="ManageFilesUIAdvanced"/>
        private delegate void NewFunctionDelegate(
            (Delegate function, object?[]? args) getDataFunction,
            (Delegate function, object?[]? args) newFileFunction,
            int maxFiles
        );

        /*/// <summary>
        /// Helper delegate for when the user clicks the new load/delete.
        /// </summary>
        /// <inheritdoc cref="ManageFilesUIAdvanced"/>*/
        //private delegate void LoadDeleteFunctionDelegate(
        //    (Delegate function, object?[]? args) getDataFunction,
        //    (Delegate function, object?[]? args) newFileFunction,
        //    (Delegate function, object?[]? args) loadFileFunction,
        //    IEnumerable<KeyAction>? keybinds,
        //    IEnumerable<object>? keyResults,
        //    string fileName,
        //    string fileExt
        //);
        #endregion
    }
}
