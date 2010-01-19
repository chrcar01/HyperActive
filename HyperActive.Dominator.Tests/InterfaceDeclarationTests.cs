using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HyperActive.Dominator.Tests
{
	[TestFixture]
	public class InterfaceDeclarationTests
	{
		[Test]
		public void CanCreateDeclaration()
		{
			InterfaceDeclaration iface = new InterfaceDeclaration("ISuck");
			new CodeBuilder().GenerateCode(Console.Out, "Blah.Blee", iface);
		}
		[Test]
		public void CanCreateInterfaceWithTypeArgs()
		{
			InterfaceDeclaration iface = new InterfaceDeclaration("ITrackPropertyChanges", new CodeDomTypeReference("Awish.Common.Models.Core.Company"));
			
			ClassDeclaration cdecl = new ClassDeclaration("Company");
			cdecl.Implements(iface);
			new CodeBuilder().GenerateCode(Console.Out, "Poops.McGee", cdecl);
		}
	}
}
