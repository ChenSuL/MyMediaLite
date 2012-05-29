// Copyright (C) 2011, 2012 Zeno Gantner
// Copyright (C) 2010 Zeno Gantner, Steffen Rendle
//
// This file is part of MyMediaLite.
//
// MyMediaLite is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MyMediaLite is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with MyMediaLite.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;

/*! \namespace MyMediaLite.Eval.Measures
*   \brief This namespace contains different evaluation measures.
*/
namespace MyMediaLite.Eval.Measures
{
	/// <summary>Area under the ROC curve (AUC) of a list of ranked items</summary>
	/// <remarks>
	/// See http://recsyswiki.com/wiki/Area_Under_the_ROC_Curve
	/// </remarks>
	public static class AUC
	{
		/// <summary>Compute the area under the ROC curve (AUC) of a list of ranked items</summary>
		/// <remarks>
		/// See http://recsyswiki.com/wiki/Area_Under_the_ROC_Curve
		/// </remarks>
		/// <param name="ranked_items">a list of ranked item IDs, the highest-ranking item first</param>
		/// <param name="relevant_items">a collection of positive/correct item IDs</param>
		/// <param name="ignore_items">a collection of item IDs which should be ignored for the evaluation</param>
		/// <returns>the AUC for the given data</returns>
		public static double Compute(IList<int> ranked_items, ICollection<int> relevant_items, ICollection<int> ignore_items = null)
		{
			if (ignore_items == null)
				ignore_items = new HashSet<int>();
			
			// TODO check again! -- think about good names
			var relevant_items_in_list = relevant_items.Intersect(ranked_items);
			int num_relevant_items = relevant_items_in_list.Count() - ignore_items.Intersect(relevant_items_in_list).Count();
			int num_eval_items     = ranked_items.Count - ignore_items.Intersect(ranked_items).Count();
			int num_eval_pairs     = (num_eval_items - num_relevant_items) * num_relevant_items;
			if (num_eval_pairs < 0)
				throw new ArgumentException("correct_items cannot be larger than ranked_items");

			if (num_eval_pairs == 0)
				return 0.5;

			int num_correct_pairs = 0;
			int hit_count         = 0;
			foreach (int item_id in ranked_items)
			{
				if (ignore_items.Contains(item_id))
					continue;

				if (!relevant_items.Contains(item_id))
					num_correct_pairs += hit_count;
				else
					hit_count++;
			}

			return (double) num_correct_pairs / num_eval_pairs;
		}
	}
}
