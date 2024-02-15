using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Contains information and bindings used to instantiate effects.
	/// </summary>
	public class EffectContext
	{
		#region Properties

		/// <summary>
		/// A text description of what triggered the effect.
		/// </summary>
		public string CauseDescription { get; }

		/// <summary>
		/// A reference to the game's social engine.
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// Preset bindings of RePraxis query variables to values.
		/// </summary>
		public Dictionary<string, object> Bindings { get; }

		/// <summary>
		/// The source of this effect.
		/// </summary>
		public IEffectSource Source { get; }

		#endregion

		#region Constructors

		public EffectContext(
			SocialEngine engine,
			string descriptionTemplate,
			Dictionary<string, object> bindings,
			IEffectSource effectSource
		)
		{
			Engine = engine;
			CauseDescription = descriptionTemplate;
			Bindings = new Dictionary<string, object>(bindings);
			Source = effectSource;

			foreach (var (variableName, value) in bindings)
			{
				CauseDescription = CauseDescription.Replace(
					$"[{variableName.Substring(1)}]", value.ToString());
			}
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="ctx"></param>
		public EffectContext(EffectContext ctx)
		{
			Engine = ctx.Engine;
			CauseDescription = ctx.CauseDescription;
			Bindings = new Dictionary<string, object>(ctx.Bindings);
			Source = ctx.Source;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Create a copy of the current context and overwrite it's bindings.
		/// </summary>
		/// <param name="bindings"></param>
		/// <returns></returns>
		public EffectContext WithBindings(Dictionary<string, object> bindings)
		{
			var updatedCtx = new EffectContext(this);

			foreach (var pair in bindings)
			{
				updatedCtx.Bindings[pair.Key] = pair.Value;
			}

			return updatedCtx;
		}

		#endregion
	}
}
