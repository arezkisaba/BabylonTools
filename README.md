# Introduction
BabylonTools est une solution contenant divers outils dont je me sers sur mon ordinateur principal afin de répondre à certains besoins spécifiques, ils seront détaillés plus bas :)

# BabylonTools.NSGSync (NonSteamGameSync)
Cet outil console me permet de synchroniser des roms **Playstation 1** et **Playstation 2** d'un dossier vers ma librairie **Steam**.

Ceci est possible grâce à la génération et l'écriture d'un fichier d'**extension .vdf** lisible par Steam. 
De cette manière ils sont accessibles comme n'importe quel jeu de Steam.

Émulateurs utilisés :
- Playstation 2 : [pcsx2](https://www.pcsx2.net/)
- Playstation 1 : [ePSXe](https://www.epsxe.com/) (pcsx2 ne supporte pas encore à 100% l'émulation de jeux Playstation 1)

# BabylonTools.Sherlock
Cet outil console m'aide principalement quand j'ai besoin de comprendre le fonctionnement d'un programme et de savoir comment récupérer telles ou telles informations depuis un service.

Par exemple, pour le projet **BabylonTools.NSGSync**, il a fallu que je sache où est stockée la liste des jeux ajoutés manuellement dans Steam afin de pouvoir la générer par la suite.
Pour se faire je lance le programme en précisant le dossier d'installation de Steam, j'ajoute un jeu manuellement depuis Steam et j'attends que le programme me dise **quels fichiers on été modifié sur le disque**.

Cela s'avère relativement pratique pour effectuer certaines tâches de **reverse engineering**.

# BabylonTools.XMap (XboxMap)
Cet outil console m'aide principalement lorsque j'utilise des **émulateurs** (Playstation 1, Playstation 2) depuis Steam en mode **BigPicture** depuis un client comme le [Nvidia Shield](https://www.nvidia.com/fr-fr/shield/).

Il me permet d'associer des actions précises à un raccourci de **manette de Xbox 360**.
Par exemple, il est impossible dans la plupart des jeux en question de quitter directement le jeu et donc de fermer l'émulateur, il faut donc dans ce cas là définir un raccourci pour effectuer l'action _quitter_.
Le but est de ne pas avoir à se servir de la souris pour faire ça et de rester tranquillement dans son canapé.

N'hésitez pas à me contacter !