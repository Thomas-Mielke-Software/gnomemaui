using System;
using System.Linq;

namespace Microsoft.Maui.Storage;

class PreferencesImplementation : IPreferences
{
	public void Clear(string? sharedName = null)
	{
		throw new NotImplementedException();
	}

	public bool ContainsKey(string key, string? sharedName = null)
	{
		throw new NotImplementedException();
	}

	public T Get<T>(string key, T defaultValue, string? sharedName = null)
	{
		throw new NotImplementedException();
	}

	public void Remove(string key, string? sharedName = null)
	{
		throw new NotImplementedException();
	}

	public void Set<T>(string key, T value, string? sharedName = null)
	{
		throw new NotImplementedException();
	}
}
