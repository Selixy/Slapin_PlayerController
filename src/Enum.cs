namespace Slapin_CharacterController
{
    public enum State
    {
        Idle,       // Au repos
        Moving,     // En mouvement
        Jumping,    // En saut
        Falling,    // En chute
        Landing,    // Au sol
        Climbing,   // En escalade
        Sliding,    // En glissade
        Attacking,  // En attaque
        Defending,  // En défense
        Dead,       // Mort
        Hurt,       // Blessé
        Stunned,    // Assommé
        Sleeping,   // En train de dormir
        Eating      // En train de manger
    }
    
    public enum BInput
    {
        Up,       // Bouton non pressé
        Down,     // Début de la pression (appui initial)
        Pressed,  // Bouton maintenu enfoncé
        Released  // Bouton relâché
    }
}
