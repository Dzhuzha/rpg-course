namespace RPG.Core
{
    public interface IPredicateEvaluator
    {
        bool? Evaluate(PredicateType predicate, string[] parameters);
        
    }

    public enum PredicateType
    {
        Select,
        HasQuest,
        CompletedObjective,
        CompletedQuest,
        HasLevel,
        MinimalTrait,
        HasItem,
        HasItems,
        HasItemEquipped
    }
}