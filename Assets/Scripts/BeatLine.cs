using UnityEngine;

/// <summary>
/// Marker component for the visual beat line spawned by <see cref="BpmSpawner"/>.
///
/// Energy recovery is intentionally NOT handled here. It lives in
/// <see cref="PlayerController.HandleEnergyRecovery"/> instead, because the beat-line
/// objects only exist in a couple of scenes (LevelBase, MechanicTest) while energy must
/// refill in every level. Driving recovery from a per-beat timer on the player keeps a
/// single, consistent source of recovery and avoids the double-recovery this script used
/// to cause where it did exist.
/// </summary>
public class BeatLine : MonoBehaviour
{
}
