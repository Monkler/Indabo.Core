namespace Indabo.Host
{
    using System;
    using System.Threading;
    using System.IO;

    using Jint;
    using Jint.Constraints;
    using Jint.Runtime;

    using Indabo.Core;

    internal class Plugin
    {
        private const int STOP_FUNCTION_TIMEOUT = 5000;

        private string filePath;

        private Engine engine;

        private Thread executionThread;

        CancellationConstraint cancellationConstraint;

        public Plugin(string filePath)
        {
            this.filePath = filePath;

            this.CreateEngine();
        }
        
        private void CreateEngine()
        {
            this.cancellationConstraint = new CancellationConstraint(new CancellationToken(false));

            this.engine = new Engine((Options options) =>
            {
                options.AllowClr(typeof(Logging).Assembly);
                options.Constraint(this.cancellationConstraint);
            });

            // TODO also add list<plugins> von pluginmanager aber als readonly
            // TODO test acces to Database
            // TODO test access to Controller
            // TODO test 
        }

        public void Start()
        {
            if (this.executionThread != null)
            {
                Logging.Error("Plugin already started! - Stop it first to restart!");
            }

            this.executionThread = new Thread(() =>
            {
                try
                {

                    this.engine.Execute(File.ReadAllText(this.filePath));
                    if (this.engine.GetValue("Start").Type == Types.Object)
                    {
                        this.engine.Invoke("Start");
                    }
                }
                catch (StatementsCountOverflowException)
                {
                    Logging.Info($"Plugin has been interrupted: '{this.filePath}'");
                }
                catch (Exception ex)
                {
                    Logging.Error($"Fatal error in Plugin: '{this.filePath}'", ex);
                }
            });

            this.executionThread.Start();
        }

        public void Stop()
        {
            Thread stopThread = new Thread(() =>
            {
                try
                {
                    if (this.engine.GetValue("Stop").Type == Types.Object)
                    {
                        this.engine.Invoke("Stop");
                    }
                }
                catch (StatementsCountOverflowException)
                {
                    Logging.Info($"Plugin has been interrupted while stopping: '{this.filePath}'");
                }
                catch (Exception ex)
                {
                    Logging.Error($"Fatal error in Plugin while stopping: '{this.filePath}'", ex);
                }
            });

            stopThread.Start();

            Thread.Sleep(STOP_FUNCTION_TIMEOUT);

            if (stopThread.IsAlive || this.executionThread.IsAlive)
            {
                this.Interupt();
            }
        }

        public void Interupt()
        {
            this.cancellationConstraint.Reset(new CancellationToken(true));
        }

        public void Profile()
        {
            // TODO - return CPU auslastung
        }
    }
}
