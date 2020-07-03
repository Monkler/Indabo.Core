namespace Indabo.Host.Windows
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Dynamically resolves the Assemblies (e.g. Plugins) 
    /// Assemblies must lay in the same directory as the executing Assembly
    /// </summary>
    public class AssemblyResolver
    {
        private bool isActive;

        private object syncRoot = new object();

        private Assembly resourceAssembly;

        /// <summary>
        /// Whether the Assembly Resolver is attached to the current app domain or not.
        /// </summary>
        public bool IsActive { get => this.isActive; set => this.isActive = value; }

        /// <summary>
        /// Attaches the Assembly Resolver to the current app domain.
        /// </summary>
        public void Activate()
        {
            lock (this.syncRoot)
            {
                if (!this.isActive)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += this.HandleAssemblyResolve;

                    this.isActive = true;
                }
            }
        }

        /// <summary>
        /// Removes the Assembly Resolver from the current app domain.
        /// </summary>
        public void Deactivate()
        {
            lock (this.syncRoot)
            {
                if (this.isActive)
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= this.HandleAssemblyResolve;

                    this.isActive = false;
                }
            }
        }

        /// <summary>
        /// Initalizes the assembly resolver using the given resource assembly.
        /// </summary>
        /// <param name="resourceAssembly">The executing assembly of the calling function. <see cref="Assembly.GetExecutingAssembly"/></param>
        public AssemblyResolver(Assembly resourceAssembly)
        {
            this.resourceAssembly = resourceAssembly;
        }

        private Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            AssemblyName assemblyName = new AssemblyName(args.Name);

            foreach (string resourceName in this.resourceAssembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(assemblyName.Name + ".dll"))
                {
                    using (Stream stream = this.resourceAssembly.GetManifestResourceStream(resourceName))
                    {
                        byte[] assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);

                        try
                        {
                            assembly = Assembly.Load(assemblyData);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Could not resolve Assembly: '{assemblyName}'. " + "Could not find File: '{directory + assemblyName.Name}.dll'", ex);
                        }
                    }
                }
            }

            return assembly;
        }
    }
}
