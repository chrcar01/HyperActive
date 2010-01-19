using System;
using HyperActive.Core;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace HyperActive.Core
{	
	/// <summary>
	/// Utility used for working with types.
	/// </summary>
	public class TypeCreator
	{
		/// <summary>
		/// Creates an instance of a type.
		/// </summary>
		/// <typeparam name="T">The type to create.</typeparam>
		/// <param name="inferer">The TypeInferer containing the necessary type information.</param>
		/// <param name="args">Any parameters needed for creating an instance of the type.</param>
		/// <returns>An instance of type T.</returns>
		public T CreateInstance<T>(TypeInferer inferer, params object[] args)
		{
			if (inferer == null)
				throw new ArgumentNullException("inferer", "inferer is null.");

			T result = default(T);
			string typeName = "";
			string assemblyName = "";
			try
			{
				typeName = inferer.TypeName.Trim();
				assemblyName = inferer.AssemblyName.Trim();
				string path1 = Path.Combine(Environment.CurrentDirectory, @"bin\" + assemblyName + ".dll");
				string path2 = Path.Combine(Environment.CurrentDirectory, @"bin\Debug\" + assemblyName + ".dll");
				Func<Assembly>[] assemblyLoaders = new Func<Assembly>[]{
					delegate { return Assembly.Load(assemblyName); },
					delegate { return Assembly.LoadFile(path1); },
					delegate { return Assembly.LoadFile(path2); }
				};
				Assembly asm = Try.These<Assembly>(assemblyLoaders);
				if (asm == null)
					throw new InvalidOperationException(String.Format("Unable to load the assembly {0} \nfrom {1} \nor {2}", assemblyName, path1, path2));

				Type type = asm.GetType(typeName, true, true);
				if (type == null)
					throw new InvalidOperationException("Unable to load type from assembly");

				result = (T)Activator.CreateInstance(type, args);
			}
			catch (Exception ex)
			{
				throw new HyperActiveException("Unable to create an instance of the type {0} from the assembly {1}", ex, typeName, assemblyName);
			}
			return result;
		}
	}
}
