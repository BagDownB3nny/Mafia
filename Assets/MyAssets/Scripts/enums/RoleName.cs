public enum RoleName
{
    RolePlaceholder, // Role that a player is set when they have not received a role yet
    // Helps with role sync var, since we want sync var to be triggered on role assignment
    Villager,
    Mafia,
    Seer,
    Guardian,
    SixthSense,

    Medium,
}