using UnityEngine;

namespace TDRS
{
	public class DecreaseRelationshipStatFactory : SocialEventEffectFactory
	{
		public class DecreaseRelationshipStat : ISocialEventEffect
		{
			protected SocialRelationship m_relationship;
			protected string m_statName;
			protected float m_value;
			protected int m_duration;
			protected string m_reason;

			public DecreaseRelationshipStat(
				SocialRelationship relationship,
				string statName,
				float value,
				int duration,
				string reason
			)
			{
				m_relationship = relationship;
				m_statName = statName;
				m_value = value;
				m_duration = duration;
				m_reason = reason;
			}

			public void Apply()
			{
				m_relationship.Stats.AddModifier(
					new StatSystem.StatModifier(
						m_statName,
						m_reason,
						-m_value,
						StatSystem.StatModifierType.FLAT,
						m_duration,
						this
					)
				);
			}

			public void Remove()
			{
				m_relationship.Stats.RemoveModifiersFromSource(this);
			}
		}

		public override string EffectName => "DecreaseRelationshipStat";

		public override ISocialEventEffect CreateInstance(EffectBindingContext ctx, params string[] args)
		{
			if (args.Length < 4)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'DecreaseRelationshipStat {argStr}'. "
					+ $"Expected at least 4 but was {args.Length}."
				);
			}

			string relationshipOwnerVar = args[0];
			string relationshipTargetVar = args[1];
			string statName = args[2];

			if (!ctx.Engine.HasRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
					)
				)
			{
				throw new System.ArgumentException(
					"No relationship found from "
					+ $"{ctx.Bindings[relationshipOwnerVar]} to"
					+ $"{ctx.Bindings[relationshipTargetVar]}."
				);
			}

			if (!float.TryParse(args[3], out var value))
			{
				throw new System.ArgumentException(
					$"Expected number as last argument but was '{args[3]}'"
				);
			}

			int duration = -1;

			if (args.Length >= 5)
			{
				if (!int.TryParse(args[4], out duration))
				{
					throw new System.ArgumentException(
						$"Expected integer as 5th argument but was '{args[4]}'"
					);
				}
			}

			return new DecreaseRelationshipStat(
				ctx.Engine.GetRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
				),
				statName,
				value,
				duration,
				ctx.Description
			);
		}
	}
}
