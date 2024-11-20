window.createToast = (toastId) => {
	if (!toastId) {
		console.error("Toast ID is missing.");
		return;
	}

	const toastElement = document.getElementById(toastId);
	if (!toastElement) {
		console.error(`Element with ID ${toastId} not found.`);
		return;
	}

	const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastElement);
	toastBootstrap.show();
}
