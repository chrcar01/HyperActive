using System;
using System.Collections.Generic;

namespace HyperActive.Core.Container
{
	/// <summary>
	/// Details about the component.
	/// </summary>
	public struct ServiceDetails
	{
		/// <summary>
		/// The service contract.
		/// </summary>
		public Type Service;

		/// <summary>
		/// The service implementation.
		/// </summary>
		public Type ServiceImpl;

		/// <summary>
		/// Constructor parameters for the service implementation.
		/// </summary>
		public List<CtorParameter> CtorParams;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDetails"/> struct.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="serviceImpl">The service impl.</param>
		/// <param name="ctorParams">The ctor params.</param>
		public ServiceDetails(Type service, Type serviceImpl, List<CtorParameter> ctorParams)
		{
			Service = service;
			ServiceImpl = serviceImpl;
            CtorParams = ctorParams ?? new List<CtorParameter>();
		}
	}
}
