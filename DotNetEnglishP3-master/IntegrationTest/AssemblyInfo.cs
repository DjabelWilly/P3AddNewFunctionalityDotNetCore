using Xunit;

// empêche xUnit de lancer les tests d’intégration en parallèle pour éviter les conflits.

[assembly: CollectionBehavior(DisableTestParallelization = true)]
