﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio;

namespace VSSetingsManager
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(VSSettingsManagerPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class VSSettingsManagerPackage : Package
    {
        /// <summary>
        /// VSSettingsManagerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "2145fd5c-c814-4772-b19d-b840113afede";
        private const string SID_SVsSettingsPersistenceManager = "9B164E40-C3A2-4363-9BC5-EB4039DEF653";
        public static ISettingsManager SettingsManager { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSSettingsManager"/> class.
        /// </summary>
        public VSSettingsManagerPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            SettingsManager = (ISettingsManager)GetGlobalService(typeof(SVsSettingsPersistenceManager));
            VSSettingsManager.Initialize(this);
            base.Initialize();

            // Check if we need to do first-time setup
            const string firstTimeRunSettingName = "SublimeSettingsPrompted";
            if ((VSSettingsManagerPackage.SettingsManager.TryGetValue(firstTimeRunSettingName, out bool value) != GetValueResult.Success) || !value)
            {
                VSSettingsManagerPackage.SettingsManager.SetValueAsync(firstTimeRunSettingName, true, isMachineLocal: true);
                ApplFirstTimeSettings();

            }
        }

        private static void ApplFirstTimeSettings()
        {
            // Apply First-time settings
            VSSettingsManager.Instance.ApplyMiniMap();
            VSSettingsManager.Instance.ApplySublimeShortcuts();
        }

        #endregion
        // A horrible hack but SVsSettingsPersistenceManager isn't public and we need something with the right GUID to get the service.
        [Guid(SID_SVsSettingsPersistenceManager)]
        private class SVsSettingsPersistenceManager
        { }
    }
}
