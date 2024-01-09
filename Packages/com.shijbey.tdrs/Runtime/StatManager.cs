using System;
using System.Collections.Generic;
using TDRS.StatSystem;

namespace TDRS
{
	/// <summary>
	/// Manages stats associated with a Entity
	/// </summary>
	public class StatManager
	{
		#region Fields

		/// <summary>
		/// Mapping of stat names to instances
		/// </summary>
		protected Dictionary<string, Stat> m_stats;

		protected List<StatModifier> m_modifiers;

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when a stat value changes
		/// </summary>
		public event EventHandler<(string, float)> OnValueChanged;

		#endregion

		#region Properties

		/// <summary>
		/// All modifiers currently applied to the stats
		/// </summary>
		public IEnumerable<StatModifier> Modifiers => m_modifiers;

		/// <summary>
		/// Get all the stat instances.
		/// </summary>
		public IEnumerable<KeyValuePair<string, Stat>> Stats => m_stats;

		#endregion

		#region Constructors

		public StatManager()
		{
			m_stats = new Dictionary<string, Stat>();
			m_modifiers = new List<StatModifier>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a new stat
		/// </summary>
		/// <param name="statName"></param>
		/// <param name="stat"></param>
		public void AddStat(string statName, Stat stat)
		{
			m_stats[statName] = stat;

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
			if (m_stats.ContainsKey(statName))
			{
				return m_stats[statName];
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
			return m_stats.ContainsKey(statName);
		}

		/// <summary>
		/// Add a modifier
		/// </summary>
		/// <param name="modifier"></param>
		public void AddModifier(StatModifier modifier)
		{
			Stat stat = GetStat(modifier.Stat);
			m_modifiers.Add(modifier);
			stat.AddModifier(modifier);
		}

		/// <summary>
		/// Remove a modifier
		/// </summary>
		/// <param name="modifier"></param>
		/// <returns></returns>
		public bool RemoveModifier(StatModifier modifier)
		{
			var success = m_modifiers.Remove(modifier);

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

			for (int i = m_modifiers.Count - 1; i >= 0; i--)
			{
				var modifier = m_modifiers[i];
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
