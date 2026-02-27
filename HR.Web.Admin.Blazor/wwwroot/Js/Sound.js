    window.playNotificationSound = () => {
        const audio = new Audio('/sounds/notification.mp3'); // Put a small mp3 file here
        audio.play().catch(e => Console.log("User must interact with page first."));
    }
