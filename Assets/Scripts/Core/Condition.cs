using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [Serializable]
    public class Condition
    {
        [SerializeField] private string _predicate;
        [SerializeField] private string[] _parameters;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (var evaluator in evaluators)
            {
                bool? result = evaluator.Evaluate(_predicate, _parameters);
                if (result == null) continue;
                if (result == false) return false;
            }

            return true;
        }
    }
}