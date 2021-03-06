//Matt Schoen
//9-27-2015
//
// This software is the copyrighted material of its author, Matt Schoen, and his company Defective Studios.
// It is available for sale on the Unity Asset store and is subject to their restrictions and limitations, as well as
// the following: You shall not reproduce or re-distribute this software without the express written (e-mail is fine)
// permission of the author. If permission is granted, the code (this file and related files) must bear this license 
// in its entirety. Anyone who purchases the script is welcome to modify and re-use the code at their personal risk 
// and under the condition that it not be included in any distribution builds. The software is provided as-is without 
// warranty and the author bears no responsibility for damages or losses caused by the software.  
// This Agreement becomes effective from the day you have installed, copied, accessed, downloaded and/or otherwise used
// the software.

//UniMerge 1.8.5
//Util class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
using PrefabUtility = UnityEditor.EditorUtility;
#endif

namespace UniMerge.Editor.Helpers {
	public static class Util {
		public static float tabSize = 15;

		public static void Foldout(ref bool open, string title, Action draw) {
			Foldout(ref open, new GUIContent(title), draw, null);
		}

		public static void Foldout(ref bool open, GUIContent content, Action draw) { Foldout(ref open, content, draw, null); }

		public static void Foldout(ref bool open, string title, Action draw, Action moreLabel) {
			Foldout(ref open, new GUIContent(title), draw, moreLabel);
		}

		public static void Foldout(ref bool open, GUIContent content, Action draw, Action moreLabel) {
			GUILayout.BeginHorizontal();
#if UNITY_5_5_OR_NEWER
			open = EditorGUILayout.Foldout(open, content, true);
#else
			open = EditorGUILayout.Foldout(open, content);
#endif
			if (moreLabel != null)
				moreLabel.Invoke();
			GUILayout.EndHorizontal();
			if (open)
				Indent(draw);
		}

		public static void Indent(Action draw) { Indent(tabSize, draw); }

		public static void Indent(float width, Action draw) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(width));
			GUILayout.Label("");
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			draw.Invoke();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		public static void Center(Action element) {
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			element();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		//Not used but a nice little gem :)
		public static void ClearLog() {
			var assembly = Assembly.GetAssembly(typeof(SceneView));

			var type = assembly.GetType("UnityEditorInternal.LogEntries");
			var method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
		}

		public static void PingButton(string content, UnityObject obj) {
			if (GUILayout.Button(content))
				EditorGUIUtility.PingObject(obj);
		}

		//Looks like Unity already has one of these... doesn't work the way I want though
		public static bool PropEqual(SerializedProperty mine, SerializedProperty theirs, GameObject myRoot,
			GameObject theirRoot) {
			if (mine != null && theirs != null)
				if (mine.propertyType == theirs.propertyType) {
					switch (mine.propertyType) {
						//TODO: update version-specific animation curve features
						case SerializedPropertyType.AnimationCurve:
							if (mine.animationCurveValue.keys.Length == theirs.animationCurveValue.keys.Length) {
								for (var i = 0; i < mine.animationCurveValue.keys.Length; i++) {
									if (mine.animationCurveValue.keys[i].inTangent != theirs.animationCurveValue.keys[i].inTangent)
										return false;

									if (mine.animationCurveValue.keys[i].outTangent != theirs.animationCurveValue.keys[i].outTangent)
										return false;

									if (mine.animationCurveValue.keys[i].time != theirs.animationCurveValue.keys[i].time)
										return false;

									if (mine.animationCurveValue.keys[i].value != theirs.animationCurveValue.keys[i].value)
										return false;
								}
							} else { return false; }
							return true;
						case SerializedPropertyType.ArraySize: break;
						case SerializedPropertyType.Boolean: return mine.boolValue == theirs.boolValue;
#if !(Unity3)
						case SerializedPropertyType.Bounds: return mine.boundsValue == theirs.boundsValue;
#endif
						case SerializedPropertyType.Character: return mine.intValue == theirs.intValue;
						case SerializedPropertyType.Color: return mine.colorValue == theirs.colorValue;
						case SerializedPropertyType.Enum: return mine.enumValueIndex == theirs.enumValueIndex;
						case SerializedPropertyType.Float: return mine.floatValue == theirs.floatValue;
						case SerializedPropertyType.Generic:
							//Override equivalence for some types that will never be equal
							switch (mine.type) {
								case "NetworkViewID": return true;
								case "GUIStyle": //Having trouble with this one
									return true;
							}
							break;
#if !(Unity3)
						case SerializedPropertyType.Gradient: break;
#endif
						case SerializedPropertyType.Integer: return mine.intValue == theirs.intValue;
						case SerializedPropertyType.LayerMask:
#if !(Unity3)
							return mine.intValue == theirs.intValue;
#else //TODO: fix layer mask compare
							return true;
#endif
						case SerializedPropertyType.ObjectReference:
							var myRef = mine.objectReferenceValue;
							var theirRef = theirs.objectReferenceValue;
							if (myRef && theirRef) {
								if (myRef.GetType() != theirRef.GetType()) {
									Debug.LogWarning("EH? two properties of different types?");
									return true;
								}

								//If the property is a gameObject or component, compare equivalence by comparing names, and whether they're both children of the same parent (from different sides)
								var myGO = myRef as GameObject;
								if (myGO && myRoot) {
									if (myGO.transform.IsChildOf(myRoot.transform)) {
										var theirGO = (GameObject) theirRef;
										if (theirGO.transform.IsChildOf(theirRoot.transform))
											return myRef.name == theirRef.name;
									}
								}

								var myComponent = myRef as Component;
								if (myComponent && myRoot) {
									if (myComponent.transform.IsChildOf(myRoot.transform)) {
										var theirComponent = (Component) theirRef;
										if (theirComponent.transform.IsChildOf(theirRoot.transform))
											return myRef.name == theirRef.name;
									}
								}
							}
							return myRef == theirRef;
						case SerializedPropertyType.Rect: return mine.rectValue == theirs.rectValue;
						case SerializedPropertyType.String: return mine.stringValue == theirs.stringValue;
						case SerializedPropertyType.Vector2: return mine.vector2Value == theirs.vector2Value;
						case SerializedPropertyType.Vector3: return mine.vector3Value == theirs.vector3Value;
#if UNITY_4_5 || UNITY_4_5_0 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
						case SerializedPropertyType.Vector4: return mine.vector4Value == theirs.vector4Value;
						case SerializedPropertyType.Quaternion: return mine.quaternionValue.Equals(theirs.quaternionValue);
#elif !Unity3
						case (SerializedPropertyType)16:
							return mine.quaternionValue.Equals(theirs.quaternionValue);
#endif
						default:
							Debug.LogWarning("Unknown SeralizedPropertyType encountered: " + mine.propertyType);
							break;
					}
				} else {
					Debug.LogWarning("Not same type?");
				}
			return true;
		}

		//U3: Had to define out some types of components. Default case will warn me about this
		//Looks like this is already implemented...
		public static void SetProperty(SerializedProperty from, SerializedProperty to) {
			switch (from.propertyType) {
				case SerializedPropertyType.AnimationCurve:
					to.animationCurveValue = from.animationCurveValue;
					break;
				case SerializedPropertyType.ArraySize:
					Debug.LogWarning("Got ArraySize type");
					break;
				case SerializedPropertyType.Boolean:
					to.boolValue = from.boolValue;
					break;
#if !Unity3
				case SerializedPropertyType.Bounds:
					to.boundsValue = from.boundsValue;
					break;
#endif
				case SerializedPropertyType.Character:
					Debug.LogWarning("Got Character type");
					break;
				case SerializedPropertyType.Color:
					to.colorValue = from.colorValue;
					break;
				case SerializedPropertyType.Enum:
					to.enumValueIndex = from.enumValueIndex;
					break;
				case SerializedPropertyType.Float:
					to.floatValue = from.floatValue;
					break;
				case SerializedPropertyType.Generic:
#if Unity3
					Debug.LogWarning("How to copy generic properties in Unity 3?");
#else
					to.serializedObject.CopyFromSerializedProperty(from);
					to.serializedObject.ApplyModifiedProperties();
#endif
					break;
#if !Unity3
				case SerializedPropertyType.Gradient:
					Debug.LogWarning("Got Gradient type");
					break;
#endif
				case SerializedPropertyType.Integer:
					to.intValue = from.intValue;
					break;
				case SerializedPropertyType.LayerMask:
					to.intValue = from.intValue;
					break;
				case SerializedPropertyType.ObjectReference:
					to.objectReferenceValue = from.objectReferenceValue;
					break;
				case SerializedPropertyType.Rect:
					to.rectValue = from.rectValue;
					break;
				case SerializedPropertyType.String:
					to.stringValue = from.stringValue;
					break;
				case SerializedPropertyType.Vector2:
					to.vector2Value = from.vector2Value;
					break;
				case SerializedPropertyType.Vector3:
					to.vector3Value = from.vector3Value;
					break;
#if UNITY_4_5 || UNITY_4_5_0 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
				case SerializedPropertyType.Vector4:
					to.vector4Value = from.vector4Value;
					break;
				case SerializedPropertyType.Quaternion:
					to.quaternionValue = from.quaternionValue;
					break;
#elif !Unity3
				case (SerializedPropertyType)16:
					to.quaternionValue = from.quaternionValue;
					break;
#endif
				default:
					Debug.LogWarning("Unknown SeralizedPropertyType encountered: " + to.propertyType);
					break;
			}
		}

		public static void GameObjectToList(GameObject obj, List<GameObject> list) {
			list.Add(obj);
			foreach (Transform t in obj.transform)
				GameObjectToList(t.gameObject, list);
		}

		/// <summary>
		///     Determine whether the object is part of a prefab, and if it is the root of the prefab tree
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public static bool IsPrefabParent(GameObject g) {
			return PrefabUtility.FindPrefabRoot(g) == g;
		}

		public static string GetPath(this Transform transform) {
			var path = transform.name;
			var parent = transform.parent;
			if (parent)
				path = parent.GetPath() + "/" + path;
			return path;
		}
	}

	public static class EditorGUILayoutExt {
		public static UnityObject ObjectField(UnityObject obj, Type objType, bool allowSceneObjects,
			params GUILayoutOption[] options) {
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3
			return EditorGUILayout.ObjectField(obj, objType, options);
#else
			return EditorGUILayout.ObjectField(obj, objType, allowSceneObjects, options);
#endif
		}

		public static UnityObject ObjectField(string label, UnityObject obj, Type objType, bool allowSceneObjects,
			params GUILayoutOption[] options) {
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3
			return EditorGUILayout.ObjectField(label, obj, objType, options);
#else
			return EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjects, options);
#endif
		}
	}
}
