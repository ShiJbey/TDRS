using System.Collections.Generic;
using TDRS.StatSystem;

namespace TDRS
{
	/// <summary>
	/// Manages stats associated with a Entity
	/// </summary>
	public class Stats
	{
		#region Attributes

		/// <summary>
		/// Mapping of stat names to instances
		/// </summary>
		protected Dictionary<string, Stat> _stats;

		protected List<StatModifier> _modifiers;

		#endregion

		#region Properties

		public IEnumerable<StatModifier> Modifiers => _modifiers;

		#endregion

		#region Constructors

		public Stats()
		{
			_stats = new Dictionary<string, Stat>();
			_modifiers = new List<StatModifier>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a new stat
		/// </summary>
		/// <param name="statName"></param>
		/// <param name="stat"></param>
		public void AddStat(string statName, Stat stat)
		{
			_stats[statName] = stat;
		}

		/// <summary>
		/// Get a reference to a stat
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		public Stat GetStat(string statName)
		{
			if (_stats.ContainsKey(statName))
			{
				return _stats[statName];
			}

			throw new System.Exception(
				$"Cannot find {statName} stat. Are you missing a stat in the inspector?");
		}

		/// <summary>
		/// Get all the stat instances.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, Stat>> GetStats()
		{
			return _stats;
		}

		/// <summary>
		/// Check if a stat exists
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		public bool HasStat(string statName)
		{
			return _stats.ContainsKey(statName);
		}

		/// <summary>
		/// Add a modifier
		/// </summary>
		/// <param name="modifier"></param>
		public void AddModifier(StatModifier modifier)
		{
			GetStat(modifier.Stat).AddModifier(modifier);
			_modifiers.Add(modifier);
		}

		/// <summary>
		/// Remove a modifier
		/// </summary>
		/// <param name="modifier"></param>
		/// <returns></returns>
		public bool RemoveModifier(StatModifier modifier)
		{
			var success = _modifiers.Remove(modifier);

			if (success)
			{
				success = GetStat(modifier.Stat).RemoveModifier(modifier);
			}

			return success;
		}

		/// <summary>
		/// Remove all modifiers from a given source
		/// </summary>
		/// <param name="source"></param>
		/// /// <returns></returns>
		public bool RemoveModifiersFromSource(object source)
		{
			bool modifierRemoved = false;

			for (int i = _modifiers.Count; i >= 0; i--)
			{
				var modifier = _modifiers[i];
				if (modifier.Source == source)
				{
					modifierRemoved = RemoveModifier(modifier);
				}
			}

			return modifierRemoved;
		}

		#endregion
	}
}
