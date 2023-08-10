using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [Serializable]
    public class Condition
    {
        [SerializeField, NonReorderable] private Disjunction[] _and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction disjunction in _and)
            {
                if (disjunction.Check(evaluators) == false)
                {
                    return false;
                }
            }

            return true;
        }

        [Serializable]
        class Disjunction
        {
            [SerializeField, NonReorderable] private Predicate[] _or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate predicate in _or)
                {
                    if (predicate.Check(evaluators)) return true;
                }

                return false;
            }
        }

        [Serializable]
        class Predicate
        {
            [SerializeField] private PredicateType _predicate;
            [SerializeField, NonReorderable] private string[] _parameters;
            [SerializeField] private bool _negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(_predicate, _parameters);
                    if (result == null) continue;
                    if (result == _negate) return false;
                }

                return true;
            }
        }
    }
}