# The Lord's Story 3D

**The Lord's Story 3D** est un RPG 3D se déroulant dans un univers médiéval.

Je suis l'auteur de l'intégralité du code présent dans ce projet.

## Systèmes mis en place

J'ai développé de nombreux systèmes pour faire fonctionner le jeu :
* **Player Controller :** Utilisation d'une *Hierarchical State Machine*.
* **EnemyController :** Gestion via une *State Machine*.
* **BossController :** *State Machine* (en cours de refonte).
* **Système d'interaction :** Mise en œuvre de l'interface `IInteractable` (Pattern Strategy).
* **PNJ :** Systèmes de dialogues, PNJ Forgeron et PNJ Marchand.
* **Système de quête :** Gestion des objectifs et de la progression.
* **Inventaire :** Système complet de gestion d'objets.
* **Équipement :** Système d'équipements pour le joueur.
* **Système de sauvegarde :** Actuellement en refonte suite à la modification du fonctionnement de l'inventaire.
* **Système de combat :** Mise en place des animations et création du script `CombatSystem` (utilisé par le joueur et les ennemis).
* **UI :** Mise en place et gestion complète du fonctionnement de l'interface utilisateur.

De nombreux systèmes uniques sont gérés en **Singleton**.

## Refonte et Transition
J'ai recréé de nombreux systèmes (situés dans le dossier `OtherNew`) afin d'assurer une transition fluide avec la refonte actuelle des systèmes du jeu.

## Informations complémentaires
* **Statut :** Projet en cours.
* **Deadline :** 28 mai 2026.
* **Vidéos :** Vous pouvez retrouver de nombreuses démonstrations des systèmes sur mon profil LinkedIn : [https://www.linkedin.com/in/lou-lefebvre-/](https://www.linkedin.com/in/lou-lefebvre-/)
