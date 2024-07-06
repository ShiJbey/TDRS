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
		/// A template text description of what triggered the effect.
		/// </summary>
		public string DescriptionTemplate { get; set; }

		/// <summary>
		/// A text description of what triggered the effect.
		/// </summary>
		public string Description
		{
			get
			{
				string description = DescriptionTemplate;

				foreach (var (variableName, value) in Bindings)
				{
					description = description.Replace(
						$"[{variableName.Substring(1)}]", value.ToString());
				}

				return description;
			}
		}

		/// <summary>
		/// A reference to the game's social engine.
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// Preset bindings of RePraxis query variables to values.
		/// </summary>
		public Dictionary<string, object> Bindings { get; }

		#endregion

		#region Constructors

		public EffectContext(
			SocialEngine engine,
			string descriptionTemplate,
			Dictionary<string, object> bindings
		)
		{
			Engine = engine;
			DescriptionTemplate = descriptionTemplate;
			Bindings = new Dictionary<string, object>(bindings);
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
			var updatedBindings = new Dictionary<string, object>(this.Bindings);
			foreach (var (key, value) in bindings)
			{
				updatedBindings[key] = value;
			}

			var updatedCtx = new EffectContext(
				this.Engine,
				this.DescriptionTemplate,
				updatedBindings
			);

			return updatedCtx;
		}

		#endregion
	}
}
