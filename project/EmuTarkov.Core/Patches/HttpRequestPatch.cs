using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Comfort.Common;
using EmuTarkov.Common.Utils.HTTP;
using EmuTarkov.Common.Utils.Patching;

namespace EmuTarkov.Core.Patches
{
	public class HttpRequestPatch : AbstractPatch
	{
		const string HTTPRequest = "Class135";
		const string HTTPHandler = "Class136";

		public HttpRequestPatch()
		{
			methodName = "smethod_0";
			flags = BindingFlags.NonPublic | BindingFlags.Static;
		}

		public override MethodInfo TargetMethod()
		{
			return PatcherConstants.TargetAssembly
				.GetTypes()
				.Single(x => x.Name == HTTPRequest).GetNestedTypes(BindingFlags.NonPublic)
				.Single(x => x.Name == HTTPHandler).GetMethod(methodName, flags);
		}

		public static bool Prefix(string url, Callback<Texture2D> callback)
		{
			Result<Texture2D> result = default;
			Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

			if (uri.IsAbsoluteUri)
			{
				result.Value = new Request(null, string.Empty).GetImage(url);
				result.Value.name = uri.Segments[uri.Segments.Length - 1];
			}
			else
			{
				return true;
			}

			callback.Invoke(result);

			return false;
		}
	}
}
