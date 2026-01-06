using System;

namespace QuestSystem
{
	/// <summary>
	/// Centralise tous les évènements du jeu.
	///
	/// Règle importante (C#): seul le type qui déclare un <c>event</c> peut l'invoquer.
	/// Les autres scripts doivent appeler les méthodes <c>Raise*</c> ci-dessous.
	/// </summary>
	public static class GameEvents
	{
		/// <summary>
		/// Déclenché quand le joueur entre dans l'eau.
		/// </summary>
		public static event Action OnPlayerEnterWater;

		/// <summary>
		/// Déclenché quand le joueur sort de l'eau.
		/// </summary>
		public static event Action OnPlayerExitWater;

		/// <summary>
		/// À appeler quand le joueur entre dans l'eau.
		/// </summary>
		public static void RaisePlayerEnterWater()
		{
			OnPlayerEnterWater?.Invoke();
		}

		/// <summary>
		/// À appeler quand le joueur sort de l'eau.
		/// </summary>
		public static void RaisePlayerExitWater()
		{
			OnPlayerExitWater?.Invoke();
		}
	}
}
