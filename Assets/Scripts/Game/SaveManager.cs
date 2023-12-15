using System;
using System.Collections.Generic;
using LL.Game.Checkpoints;
using LL.Game.Equipment;
using LL.Game.Story;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveManager : MonoBehaviour
{
    public const string INVENTORY_KEY = "inventory";
    public const string LEVEL_KEY_PREFIX = "level_";
    public const string CHECKPOINT_KEY = "checkpoint";

    [Serializable]
    public struct EquippedInSlot
    {
        public string slot;
        public string? piece;
    }

    [Serializable]
    public struct InventorySave
    {
        public List<EquippedInSlot> equipped;
        public List<string> inventory;
    }

    void OnEnable()
    {
        Load();
    }

    static void SaveInventory()
    {
        var inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryBehavior>();

        var equipment = inventory.CurrentEquipment;

        List<EquippedInSlot> equipped = new();

        var slots = equipment.Container.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            var piece = equipment.GetAtIndex(i);
            equipped.Add(new EquippedInSlot
            {
                slot = slots[i].Slot.SlotName,
                piece = piece != null ? piece.name : null,
            });
        }

        List<string> inventoryItems = new();

        foreach (var piece in inventory.GetView())
        {
            inventoryItems.Add(piece.name);
        }

        var inventoryJson = JsonUtility.ToJson(new InventorySave
        {
            equipped = equipped,
            inventory = inventoryItems,
        });

        PlayerPrefs.SetString(INVENTORY_KEY, inventoryJson);
    }

    [Serializable]
    public struct LevelSave
    {
        public List<string> inactiveDialogs;
        public List<string> inactiveItemPickups;
    }

    [Serializable]
    public struct CheckpointSave
    {
        public string? checkpointGameObject;
        public Vector2 checkpoint;
        public string level;
    }

    static void SaveLevel()
    {
        var dialogs = FindObjectsByType<DialogTrigger>(FindObjectsSortMode.None);

        List<string> inactiveDialogs = new();

        foreach (var dialog in dialogs)
        {
            if (!dialog.CanShow)
            {
                inactiveDialogs.Add(dialog.gameObject.name);
            }
        }

        var equipmentPickups = FindObjectsByType<EquipmentPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        List<string> inactiveItemPickups = new();

        foreach (var pickup in equipmentPickups)
        {
            if (!pickup.isActiveAndEnabled)
            {
                inactiveItemPickups.Add(pickup.gameObject.name);
            }
        }

        var levelJson = JsonUtility.ToJson(new LevelSave
        {
            inactiveDialogs = inactiveDialogs,
            inactiveItemPickups = inactiveItemPickups,
        });

        var levelKey = LEVEL_KEY_PREFIX + SceneManager.GetActiveScene().name;

        PlayerPrefs.SetString(levelKey, levelJson);
    }

    static void SaveCheckpoint()
    {
        var checkpointable = GameObject.FindGameObjectWithTag("Player").GetComponent<Checkpointable>();

        var checkpointJson = JsonUtility.ToJson(new CheckpointSave
        {
            checkpoint = checkpointable.Checkpoint,
            level = SceneManager.GetActiveScene().name,
        });

        PlayerPrefs.SetString(CHECKPOINT_KEY, checkpointJson);
    }

    static void SaveCheckpointOverride(string level, string checkpointGameObject)
    {
        var checkpointJson = JsonUtility.ToJson(new CheckpointSave
        {
            checkpointGameObject = checkpointGameObject,
            level = level,
        });

        PlayerPrefs.SetString(CHECKPOINT_KEY, checkpointJson);
    }

    static void LoadInventory()
    {
        var inventoryJson = PlayerPrefs.GetString(INVENTORY_KEY);

        if (string.IsNullOrEmpty(inventoryJson))
            return;

        var savedInventory = JsonUtility.FromJson<InventorySave>(inventoryJson);

        var inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryBehavior>();

        var equipment = inventory.CurrentEquipment;

        for (int i = 0; i < savedInventory.equipped.Count; i++)
        {
            var pieceKey = savedInventory.equipped[i].piece;
            if (string.IsNullOrEmpty(pieceKey))
                continue;
            var piece = EquipmentDatabase.GetFromAssetName(pieceKey);
            if (piece == null)
            {
                Debug.LogWarning($"Could not find piece {pieceKey} in Equipment database!");
                continue;
            }
            equipment.EquipAtIndex(piece, i);
        }

        foreach (var pieceKey in savedInventory.inventory)
        {
            var piece = EquipmentDatabase.GetFromAssetName(pieceKey);
            if (piece == null)
            {
                Debug.LogWarning($"Could not find piece {pieceKey} in Equipment database!");
                continue;
            }
            inventory.AddToInventory(piece);
        }
    }

    static void LoadLevel()
    {
        var levelKey = LEVEL_KEY_PREFIX + SceneManager.GetActiveScene().name;
        var levelJson = PlayerPrefs.GetString(levelKey);

        if (string.IsNullOrEmpty(levelJson))
            return;

        var savedLevel = JsonUtility.FromJson<LevelSave>(levelJson);

        var dialogs = FindObjectsByType<DialogTrigger>(FindObjectsSortMode.None);

        foreach (var dialog in dialogs)
        {
            if (savedLevel.inactiveDialogs.Contains(dialog.gameObject.name))
            {
                dialog.CanShow = false;
            }
        }

        var equipmentPickups = FindObjectsByType<EquipmentPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var pickup in equipmentPickups)
        {
            if (savedLevel.inactiveItemPickups.Contains(pickup.gameObject.name))
            {
                pickup.gameObject.SetActive(false);
            }
        }

        PlayerPrefs.SetString(levelKey, levelJson);
    }

    static void LoadCheckpoint()
    {
        var checkpointJson = PlayerPrefs.GetString(CHECKPOINT_KEY);

        if (string.IsNullOrEmpty(checkpointJson))
            return;

        var savedCheckpoint = JsonUtility.FromJson<CheckpointSave>(checkpointJson);

        if (savedCheckpoint.level == SceneManager.GetActiveScene().name)
        {
            var checkpointable = GameObject.FindGameObjectWithTag("Player").GetComponent<Checkpointable>();

            checkpointable.SetCheckpoint(savedCheckpoint.checkpoint);

            if (!string.IsNullOrEmpty(savedCheckpoint.checkpointGameObject))
            {
                var go = GameObject.Find(savedCheckpoint.checkpointGameObject);
                if (go != null)
                {
                    checkpointable.SetCheckpoint(go.transform.position);
                }
            }

            checkpointable.ResetToCheckpoint();
        }
#if UNITY_EDITOR
        else
        {
            if (Application.isEditor)
            {
                var result = EditorUtility.DisplayDialogComplex(
                    "Error loading checkpoint",
                    "Checkpoint level does not match",
                    "Reset checkpoint",
                    "Do nothing",
                    "Load level"
                );
                switch (result)
                {
                    case 0:
                        PlayerPrefs.DeleteKey(CHECKPOINT_KEY);
                        break;
                    case 1:
                        break;
                    case 2:
                        SceneManager.LoadSceneAsync(savedCheckpoint.level, LoadSceneMode.Single);
                        break;
                }
            }
        }
#endif
    }

#if UNITY_EDITOR
    [MenuItem("Tools/LL/Saves/Save")]
#endif
    public static void Save()
    {
        SaveInventory();
        SaveLevel();
        SaveCheckpoint();
    }

    public static void SaveWithCheckpointOverride(string level, string checkpointGameObject)
    {
        SaveInventory();
        SaveLevel();
        SaveCheckpointOverride(level, checkpointGameObject);
    }

#if UNITY_EDITOR
    [MenuItem("Tools/LL/Saves/Load")]
#endif
    public static void Load()
    {
        LoadInventory();
        LoadLevel();
        LoadCheckpoint();
    }

#if UNITY_EDITOR
    [MenuItem("Tools/LL/Saves/Reset")]
#endif
    public static void Reset()
    {
        PlayerPrefs.DeleteAll();
    }
}
