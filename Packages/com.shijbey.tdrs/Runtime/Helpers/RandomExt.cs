using System;
using System.Collections.Generic;
using System.Linq;

namespace TDRS.Helpers
{
	public static class RandomExt
	{
		public static List<T> Sample<T>(this System.Random rand, IEnumerable<T> population, int sampleSize)
		{
			List<T> populationList = population.ToList();
			List<T> sample = new List<T>();

			if (sampleSize > populationList.Count)
			{
				throw new ArgumentException("Sample size cannot be larger than population size.");
			}

			for (int i = 0; i < sampleSize; i++)
			{
				int index = rand.Next(0, populationList.Count);
				sample.Add(populationList[index]);
				populationList.RemoveAt(index);
			}

			return sample;
		}

		public static T Choice<T>(this System.Random rand, IEnumerable<T> population)
		{
			List<T> populationList = population.ToList();

			if (populationList.Count == 0)
			{
				throw new ArgumentException("Collection is empty.");
			}

			int index = rand.Next(0, populationList.Count);

			return populationList[index];
		}
	}
}
