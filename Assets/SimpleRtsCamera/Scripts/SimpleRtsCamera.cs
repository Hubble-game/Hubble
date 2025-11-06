using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleRtsCamera.Scripts
{
	public class SimpleRtsCamera : MonoBehaviour
	{
		[Header("RTS Camera Settings")]
		[Header("Move")]
		[SerializeField] private float _moveSpeed = 200;
		[SerializeField] private float _edgeThreshold = 5;
		[SerializeField] private float _rightMouseSpeedMultiplier = 10;

		[Header("Zoom")]
		[SerializeField] private float _zoomSpeed = 200;

		[Header("Rotate")]
		[SerializeField] private float _rotateSpeed = 0.5f;
		[SerializeField] private float _rotateFallback = 1000f;

		private PlayerInput _playerInput;
		private Vector2 _moveInput;
		private Vector2 _mousePositionInput;
		private float _rightMouseInput;
		private Vector2 _initialMousePosition;
		private float _scrollMouseInput;
		private float _middleMouseInput;

		private void Awake()
		{
			_playerInput = FindAnyObjectByType<PlayerInput>();
		}

		private void OnEnable()
		{
			_playerInput.actions["CameraMove"].performed += MoveHandler;
			_playerInput.actions["CameraMove"].canceled += MoveHandler;

			_playerInput.actions["MousePosition"].performed += MousePositionHandler;
			_playerInput.actions["MousePosition"].canceled += MousePositionHandler;

			_playerInput.actions["RightMouse"].started += InitialMousePositionHandler;
			_playerInput.actions["RightMouse"].performed += RightMouseHandler;
			_playerInput.actions["RightMouse"].canceled += RightMouseHandler;

			_playerInput.actions["ScrollMouse"].performed += ScrollMouseHandler;
			_playerInput.actions["ScrollMouse"].canceled += ScrollMouseHandler;

			_playerInput.actions["MiddleMouse"].started += InitialMousePositionHandler;
			_playerInput.actions["MiddleMouse"].performed += MiddleMouseHandler;
			_playerInput.actions["MiddleMouse"].canceled += MiddleMouseHandler;
		}

		private void LateUpdate()
		{
			MoveCamera();
			MoveCameraWithCursor();
			MoveCameraWithRightMouse();
			ZoomCamera();
			RotateCamera();
		}

		private void OnDisable()
		{
			if (!_playerInput) return;

			_playerInput.actions["CameraMove"].performed -= MoveHandler;
			_playerInput.actions["CameraMove"].canceled -= MoveHandler;

			_playerInput.actions["MousePosition"].performed -= MousePositionHandler;
			_playerInput.actions["MousePosition"].canceled -= MousePositionHandler;

			_playerInput.actions["RightMouse"].started -= InitialMousePositionHandler;
			_playerInput.actions["RightMouse"].performed -= RightMouseHandler;
			_playerInput.actions["RightMouse"].canceled -= RightMouseHandler;

			_playerInput.actions["ScrollMouse"].performed -= ScrollMouseHandler;
			_playerInput.actions["ScrollMouse"].canceled -= ScrollMouseHandler;

			_playerInput.actions["MiddleMouse"].started -= InitialMousePositionHandler;
			_playerInput.actions["MiddleMouse"].performed -= MiddleMouseHandler;
			_playerInput.actions["MiddleMouse"].canceled -= MiddleMouseHandler;
		}

		private void MoveHandler(InputAction.CallbackContext callbackContext) =>
			_moveInput = callbackContext.ReadValue<Vector2>();

		private void MousePositionHandler(InputAction.CallbackContext callbackContext) =>
			_mousePositionInput = callbackContext.ReadValue<Vector2>();

		private void InitialMousePositionHandler(InputAction.CallbackContext callbackContext) =>
			_initialMousePosition = _mousePositionInput;

		private void RightMouseHandler(InputAction.CallbackContext callbackContext) =>
			_rightMouseInput = callbackContext.ReadValue<float>();

		private void ScrollMouseHandler(InputAction.CallbackContext callbackContext) =>
			_scrollMouseInput = callbackContext.ReadValue<float>();

		private void MiddleMouseHandler(InputAction.CallbackContext callbackContext) =>
			_middleMouseInput = callbackContext.ReadValue<float>();

		private void MoveCamera()
		{
			var moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y) * (_moveSpeed * Time.deltaTime);
			transform.position += moveDirection;
		}

		private void MoveCameraWithCursor()
		{
			if (_mousePositionInput.x < _edgeThreshold)
			{
				transform.position += Vector3.left * (_moveSpeed * Time.deltaTime);
			}
			else if (_mousePositionInput.x > Screen.width - _edgeThreshold)
			{
				transform.position += Vector3.right * (_moveSpeed * Time.deltaTime);
			}

			if (_mousePositionInput.y < _edgeThreshold)
			{
				transform.position += Vector3.back * (_moveSpeed * Time.deltaTime);
			}
			else if (_mousePositionInput.y > Screen.height - _edgeThreshold)
			{
				transform.position += Vector3.forward * (_moveSpeed * Time.deltaTime);
			}
		}

		private void MoveCameraWithRightMouse()
		{
			if (Mathf.Approximately(_rightMouseInput, 0) || _initialMousePosition == Vector2.zero) return;

			var mouseDelta = _mousePositionInput - _initialMousePosition;

			var moveX = mouseDelta.x * (_moveSpeed * _rightMouseSpeedMultiplier / Screen.width) * Time.deltaTime;
			var moveY = mouseDelta.y * (_moveSpeed * _rightMouseSpeedMultiplier / Screen.height) * Time.deltaTime;

			var moveDirection = new Vector3(moveX, 0, moveY);
			transform.position += moveDirection;
		}

		private void ZoomCamera()
		{
			if (Mathf.Approximately(_scrollMouseInput, 0)) return;

			var zoomDirection = transform.forward;
			transform.position += zoomDirection * (_scrollMouseInput * _zoomSpeed * Time.deltaTime);
		}

		private void RotateCamera()
		{
			if (Mathf.Approximately(_middleMouseInput, 0)) return;

			var lookAtPoint = GetCameraLookAtPoint();
			var mouseDelta = _mousePositionInput - _initialMousePosition;
			transform.RotateAround(lookAtPoint, Vector3.up, mouseDelta.x * _rotateSpeed * Time.deltaTime);
		}

		private Vector3 GetCameraLookAtPoint()
		{
			var ray = new Ray(transform.position, transform.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
			{
				return hit.point;
			}

			var groundPlane = new Plane(Vector3.up, Vector3.zero);
			if (groundPlane.Raycast(ray, out float enter))
			{
				return ray.GetPoint(enter);
			}

			return transform.position + transform.forward * _rotateFallback;
		}
	}
}