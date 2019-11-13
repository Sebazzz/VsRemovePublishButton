//------------------------------------------------------------------------------
// <copyright file="VsRemovePublishButtonPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VsRemovePublishButton {
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Application = System.Windows.Application;
    using Task = System.Threading.Tasks.Task;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionPageCustom), "Environment", "Status Bar Publish Button", 0, 0, true)]
    public sealed class VsRemovePublishButtonPackage : AsyncPackage, IVsSolutionEvents {
        public const string PackageGuidString = "967c2ebc-beef-44c3-bd8e-83da5f654bae";

        private const int MaximumRetries = 50;

        private int _retryCount;

        protected override async Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            await this.DoHidePublishButtonIfRequiredAsync(cancellationToken);

            await this.SetupEventHandlingAsync(cancellationToken);

            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(false);
        }

        private async Task DoHidePublishButtonIfRequiredAsync(System.Threading.CancellationToken cancellationToken) {
            if (ThreadHelper.CheckAccess() == false) {
                await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }

            OptionPageCustom page = (OptionPageCustom) this.GetDialogPage(typeof(OptionPageCustom));

            if (page.HideByDefault) {

                this.DoHidePublishButton();
            }
        }

        private async Task SetupEventHandlingAsync(System.Threading.CancellationToken cancellationToken) {
            if (ThreadHelper.CheckAccess() == false) {
                await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }

            IVsSolution solution = (IVsSolution) await this.GetServiceAsync(typeof(SVsSolution));
            
            if (solution == null) {
                this.LogWarning($"Unable to listen for solution events: this.GetServiceAsync(typeof(SVsSolution)) returned null");
                return;
            }

            int hr;
            if ((hr = solution.AdviseSolutionEvents(this, out _)) != VSConstants.S_OK) {
                this.LogWarning($"Unable to listen for solution events: HRESULT {hr:x2}");
            }
        }

        private void DoHidePublishButton() {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (PublishButton.TryHide()) {
                this._retryCount = 0;
                return;
            }

            if (this._retryCount++ > MaximumRetries) {
                this._retryCount = 0;

                this.LogWarning("Was not able to find publish button");
                return;
            }

#if DEBUG
            this.LogInfo("Not able to find publish button - retrying later");
#endif 

            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher) {
                Interval = TimeSpan.FromMilliseconds(1000),
            };

            timer.Tick += this.TryHidePublishButton;

            timer.Start();
        }

        private void TryHidePublishButton(object sender, EventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();

            DispatcherTimer timer = (DispatcherTimer) sender;

            timer.Stop();

            this.DoHidePublishButton();
        }

        private void LogWarning(string text) {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsActivityLog log = this.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            if (log == null) return;

            log.LogEntry((UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING,
                this.ToString(),
                text);
        }

        private void LogInfo(string text) {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsActivityLog log = this.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            if (log == null) return;

            log.LogEntry((UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
                this.ToString(),
                text);
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            this.JoinableTaskFactory.RunAsync(async () => {
                await this.JoinableTaskFactory.SwitchToMainThreadAsync();

                await this.DoHidePublishButtonIfRequiredAsync(CancellationToken.None);
            });

            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) => VSConstants.S_OK;

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) => VSConstants.S_OK;

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) => VSConstants.S_OK;

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) => VSConstants.S_OK;

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) => VSConstants.S_OK;

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) => VSConstants.S_OK;

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)  => VSConstants.S_OK;

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved) => VSConstants.S_OK;

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved) => VSConstants.S_OK;
    }

    [Guid("3993FEBB-7965-4EEB-A071-D20788FB37AF")]
    public class OptionPageCustom : DialogPage {
        private OptionPage _optionsPageCtrl;

        public bool HideByDefault { get; set; } = true;

        protected override IWin32Window Window => this.CreateSettingsControl();

        private IWin32Window CreateSettingsControl() {
            return this._optionsPageCtrl = new OptionPage(this);
        }

        protected override void OnActivate(CancelEventArgs e) {
            base.OnActivate(e);

            this._optionsPageCtrl?.OnActivate();
        }
    }
}