using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.KillScript.Core.Module
{
    /// <summary>
    /// Encapsulates arguments for a game module's switch event.
    /// </summary>
    public class ModuleSwitchEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates an instance of ModuleExitEventArgs with a null directive.
        /// </summary>
        public ModuleSwitchEventArgs() { }

        //Createst and instance of the ModuleExitEventArgs with the specified d
        public ModuleSwitchEventArgs(string key)
        {
            Key = key;
        }
        #endregion
        /// <summary>
        /// Specifices the reason for which the EXIT event occured.
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// Represents the reference to a method handler which handles a game module's "Exit" event.
    /// </summary>
    /// <param name="sender">The object which generated the event. Typically, the game module.</param>
    /// <param name="args">The arguments for the event.</param>
    public delegate void ModuleSwitchEventHandler(object sender, ModuleSwitchEventArgs args);
}
