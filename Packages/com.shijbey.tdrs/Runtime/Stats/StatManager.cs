using System;
using System.Collections.Generic;

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

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when a stat value changes
		/// </summary>
		public event EventHandler<OnValueChangedArgs> OnValueChanged;
		public class OnValueChangedArgs : EventArgs
		{
			public string StatName { get; }
			public float Value { get; }

			public OnValueChangedArgs(string statName, float value)
			{
				StatName = statName;
				Value = value;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get all the stat instances.
		/// </summary>
		public IEnumerable<KeyValuePair<string, Stat>> Stats => m_stats;

		#endregion

		#region Constructors

		public StatManager()
		{
			m_stats = new Dictionary<string, Stat>();
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

			stat.OnValueChanged += (stat, args) =>
			{
				OnValueChanged?.Invoke(this, new OnValueChangedArgs(statName, args.Value));
			};

			OnValueChanged?.Invoke(this, new OnValueChangedArgs(statName, stat.Value));
		}

		/// <summary>
		/// Get a reference to a stat
		/// </summary>
		/// <param name="statName"></param>
		/// <returns></returns>
		public Stat GetStat(string statName)
		{
			return m_stats[statName];
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

		#endregion
	}
}
