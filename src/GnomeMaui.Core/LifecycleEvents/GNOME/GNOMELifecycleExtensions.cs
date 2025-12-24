using System;

namespace Microsoft.Maui.LifecycleEvents;

public static class GNOMELifecycleExtensions
{
	public static ILifecycleBuilder AddGnome(this ILifecycleBuilder builder, Action<IGNOMELifecycleBuilder> configureDelegate)
	{
		var lifecycleBuilder = new LifecycleBuilder(builder);

		configureDelegate?.Invoke(lifecycleBuilder);

		return builder;
	}

	class LifecycleBuilder(ILifecycleBuilder builder) : IGNOMELifecycleBuilder
	{
		readonly ILifecycleBuilder _builder = builder;

		public void AddEvent<TDelegate>(string eventName, TDelegate action) where TDelegate : Delegate =>
			_builder.AddEvent(eventName, action);
	}
}