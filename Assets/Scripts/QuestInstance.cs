[System.Serializable]
public class QuestInstance
{
    public QuestSO data;          // La définition de la quête
    public QuestStatus status;    // Etat actuel
    public int currentCount;      // Compteur de progression (objets collectés, kills…)
    public bool interactionDone;  // Pour les quêtes d'interaction
    public bool escortFinished; // Pour les quêtes d'escorte
}
