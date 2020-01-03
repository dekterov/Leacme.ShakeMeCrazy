// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

		GetNode<MeshInstance>("ShakeSmiley/Body").MaterialOverride = new SpatialMaterial {
			AlbedoColor = Color.FromHsv(0.25f, 1, 1)
		};

	}

	public override void _Process(float delta) {

		var accelVec = Input.GetAccelerometer() - Input.GetGravity();
		var accelSum = Math.Abs(accelVec.x + accelVec.y + accelVec.z);
		var mouth = GetNode("ShakeSmiley").GetNode("Body").GetNode<MeshInstance>("Mouth");
		if (accelSum > 5) {

			GetNode("ShakeSmiley").GetNode("Body").GetNode<RigidBody>("EyeLPhysics").ApplyTorqueImpulse(new Vector3(0, -(float)GD.RandRange(0.01f, accelSum / 100.0f), 0));
			GetNode("ShakeSmiley").GetNode("Body").GetNode<RigidBody>("EyeRPhysics").ApplyTorqueImpulse(new Vector3(0, (float)GD.RandRange(0.01f, accelSum / 100.0f), 0));
			if (mouth.Scale.x < 1) {
				GetNode<Camera>("cam").HOffset = (float)GD.RandRange(-1.0, 1.0) * 0.2f;
				GetNode<Camera>("cam").VOffset = (float)GD.RandRange(-1.0, 1.0) * 0.2f;
				var tempScale = mouth.Scale;
				tempScale.x += (delta / 2);
				mouth.Scale = tempScale;
				((SpatialMaterial)GetNode<MeshInstance>("ShakeSmiley/Body").MaterialOverride).AlbedoColor.ToHsv(out var hue, out var sat, out var value);
				GetNode<MeshInstance>("ShakeSmiley/Body").MaterialOverride = new SpatialMaterial {
					AlbedoColor = Color.FromHsv(hue + 0.033f * delta / 2 * 10, 1, 1)
				};
			}
		} else if (mouth.Scale.x > 0.13) {
			var tempScale = mouth.Scale;
			tempScale.x += (-delta / 15);
			mouth.Scale = tempScale;
			((SpatialMaterial)GetNode<MeshInstance>("ShakeSmiley/Body").MaterialOverride).AlbedoColor.ToHsv(out var hue, out var sat, out var value);
			GetNode<MeshInstance>("ShakeSmiley/Body").MaterialOverride = new SpatialMaterial {
				AlbedoColor = Color.FromHsv(hue - 0.033f * delta / 15 * 10, 1, 1)
			};
		}

	}
}
