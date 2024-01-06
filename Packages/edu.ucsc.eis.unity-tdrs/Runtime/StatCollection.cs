using System;
using System.Collections.Generic;
using TDRS.StatSystem;

namespace TDRS
{
	/// <summary>
	/// Manages stats associated with a Entity
	/// </summary>
	public class StatCollection
	{
		#region Attributes

		/// <summary>
		/// Mapping of stat names to instances
		/// </summary>
		protected Dictionary<string, Stat> _stats;

		protected List<StatModifier> _modifiers;

		#endregion

		#region Events

		public event EventHandler<(string, float)> OnValueChanged;

		#endregion

		#region Properties

		public IEnumerable<StatModifier> Modifiers => _modifiers;

		/// <summary>
		/// Get all the stat instances.
		/// </summary>
		public IEnumerable<KeyValuePair<string, Stat>> Stats => _stats;

		#endregion

		#region Constructors

		public StatCollection()
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

			stat.OnValueChanged += (stat, value) =>
			{
				if (OnValueChanged != null) OnValueChanged.Invoke(this, (statName, value));
			};

			if (OnValueChanged != null) OnValueChanged.Invoke(this, (statName, stat.Value));
		}

		/// <summary>
		/// Get a reference to a stat
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public Stat GetStat(string statName)
		{
			if (_stats.ContainsKey(statName))
			{
				return _stats[statName];
			}

			throw new KeyNotFoundException(
				$"Cannot find {statName} stat. Are you missing a stat in the inspector?");
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
			Stat stat = GetStat(modifier.Stat);
			_modifiers.Add(modifier);
			stat.AddModifier(modifier);
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
				Stat stat = GetStat(modifier.Stat);
				success = stat.RemoveModifier(modifier);
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

			for (int i = _modifiers.Count - 1; i >= 0; i--)
			{
				var modifier = _modifiers[i];
				if (modifier.Source == source)
				{
					modifierRemoved = RemoveModifier(modifier);
				}
			}

			return modifierRemoved;
		}

		/// <summary>
		/// Update the stats and modifiers by one simulation tick
		/// </summary>
		public void Tick()
		{
			// Loop backward since we may remove items from the list
			for (int i = _modifiers.Count - 1; i >= 0; i--)
			{
				var modifier = _modifiers[i];

				if (modifier.Duration > 0)
				{
					modifier.DecrementDuration();
				}

				if (modifier.Duration == 0)
				{
					RemoveModifier(modifier);
				}
			}
		}

		#endregion
	}
}
