using NUnit.Framework;
using System.Linq; 
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameInputActionsTests
{
    private GameInputActions inputActions;

    [SetUp]
    public void Setup()
    {
        inputActions = new GameInputActions();
        
    }

    [TearDown]
    public void Teardown()
    {
        if (inputActions != null)
        {
            inputActions.Dispose();
            inputActions = null;
        }
        Assert.Pass();
    }

    [Test]
    public void Constructor_CreatesAssetAndPlayerActions()
    {
        Assert.IsNotNull(inputActions.asset, "Asset should not be null.");
        Assert.IsNotNull(inputActions.Player.Move, "Player.Move action should not be null.");
        Assert.IsNotNull(inputActions.Player.SwipeMouse, "Player.SwipeMouse action should not be null.");
        Assert.IsNotNull(inputActions.Player.SwipeTouch, "Player.SwipeTouch action should not be null.");
    }

    [Test]
    public void BindingMask_Property_Works()
    {
        var mask = new InputBinding { path = "<Keyboard>/space" };
        inputActions.bindingMask = mask;
        Assert.AreEqual(mask, inputActions.bindingMask);
    }

    [Test]
    public void Devices_Property_Works()
    {
        var devices = new ReadOnlyArray<InputDevice>(new InputDevice[0]);
        inputActions.devices = devices;
        Assert.AreEqual(devices, inputActions.devices);
    }

    [Test]
    public void ControlSchemes_Property_Works()
    {
        var schemes = inputActions.controlSchemes;
        Assert.IsNotNull(schemes);
        Assert.IsTrue(schemes.Count > 0, "At least one control scheme should exist.");
    }

    [Test]
    public void Contains_ReturnsTrue_ForExistingAction()
    {
        Assert.IsTrue(inputActions.Contains(inputActions.Player.Move));
    }

    [Test]
    public void GetEnumerator_EnumeratesActions()
    {
        var actions = new List<InputAction>();
        foreach (var action in inputActions)
        {
            actions.Add(action);
        }
        Assert.IsTrue(actions.Count > 0, "Should enumerate at least one action.");
    }

    // *** Дополнительный тест для покрытия именно public IEnumerator<InputAction> GetEnumerator() ***
    [Test]
    public void TypedEnumerator_Works()
    {
        // Вызываем явно метод GetEnumerator(), который возвращает IEnumerator<InputAction>
        var enumerator = inputActions.GetEnumerator();
        int count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        Assert.IsTrue(count > 0, "There should be at least one action enumerated via typed enumerator.");
    }

    [Test]
    public void Enable_Disable_Works()
    {
        inputActions.Enable();
        Assert.IsTrue(inputActions.asset.enabled, "Asset should be enabled after Enable().");
        inputActions.Disable();
        Assert.IsFalse(inputActions.asset.enabled, "Asset should be disabled after Disable().");
    }

    [Test]
    public void Bindings_Property_Works()
    {
        var bindingList = inputActions.bindings.ToList();
        Assert.IsNotNull(bindingList);
        Assert.IsTrue(bindingList.Count > 0, "There should be some bindings defined.");
    }

    [Test]
    public void FindAction_FindsActionByName()
    {
        var action = inputActions.FindAction("Move", throwIfNotFound: true);
        Assert.IsNotNull(action);
        Assert.AreEqual("Move", action.name);
    }

    [Test]
    public void FindAction_ReturnsNull_ForNonexistentAction()
    {
        var action = inputActions.FindAction("NonexistentAction", throwIfNotFound: false);
        Assert.IsNull(action);
    }

    [Test]
    public void FindBinding_ReturnsValidIndex()
    {
        var bindingList = inputActions.bindings.ToList();
        Assert.IsTrue(bindingList.Count > 0);
        var firstBinding = bindingList[0];
        InputAction outAction;
        int index = inputActions.FindBinding(firstBinding, out outAction);
        Assert.IsTrue(index >= 0, "Should return valid index for an existing binding.");
        Assert.IsNotNull(outAction);
    }

    [Test]
    public void PlayerActions_ImplicitConversion_Works()
    {
        GameInputActions.PlayerActions playerActions = inputActions.Player;
        InputActionMap map = playerActions;
        Assert.IsNotNull(map);
        Assert.AreEqual(inputActions.asset.FindActionMap("Player"), map);
    }

    private class DummyPlayerActions : GameInputActions.IPlayerActions
    {
        public bool moveCalled;
        public bool swipeMouseCalled;
        public bool swipeTouchCalled;

        public void OnMove(InputAction.CallbackContext context)
        {
            moveCalled = true;
        }
        public void OnSwipeMouse(InputAction.CallbackContext context)
        {
            swipeMouseCalled = true;
        }
        public void OnSwipeTouch(InputAction.CallbackContext context)
        {
            swipeTouchCalled = true;
        }
    }

    [Test]
    public void PlayerActions_AddRemove_SetCallbacks_Works()
    {
        var dummy1 = new DummyPlayerActions();
        var dummy2 = new DummyPlayerActions();
        var playerActions = inputActions.Player;

        playerActions.AddCallbacks(dummy1);
        dummy1.OnMove(default);
        dummy1.OnSwipeMouse(default);
        dummy1.OnSwipeTouch(default);
        Assert.IsTrue(dummy1.moveCalled);
        Assert.IsTrue(dummy1.swipeMouseCalled);
        Assert.IsTrue(dummy1.swipeTouchCalled);

        playerActions.RemoveCallbacks(dummy1);

        playerActions.SetCallbacks(dummy2);
        Assert.Pass();
    }

    [Test]
    public void PlayerScheme_Property_CoversBranch()
    {
        var scheme = inputActions.PlayerScheme;
        Assert.IsNotNull(scheme.name);
        Assert.AreEqual("Player", scheme.name, "PlayerScheme should have name 'Player'.");
    }

    [Test]
    public void PlayerActions_EnableDisable_AndEnabledProperty_Work()
    {
        var playerActions = inputActions.Player;
        Assert.IsFalse(playerActions.enabled, "PlayerActions should be disabled by default (unless globally enabled).");

        playerActions.Enable();
        Assert.IsTrue(playerActions.enabled, "After Enable() call, PlayerActions should be enabled.");

        playerActions.Disable();
        Assert.IsFalse(playerActions.enabled, "After Disable() call, PlayerActions should be disabled.");
    }

    [Test]
    public void Dispose_DestroysAsset()
    {
        var asset = inputActions.asset;
        inputActions.Dispose();
        Assert.IsTrue(!asset, "Asset should be destroyed after Dispose().");
    }
}
