window.scrollToBottom = (elementId) => {
    const element = document.getElementById(elementId);
    if (!element) return;

    var targetScrollTop = element.scrollHeight - element.clientHeight;
    const scrollSpeed = 10; // Justera hastigheten för scrollning här

    // Definiera scrollStep som en funktion
    const scrollStep = () => {
        const currentScrollTop = element.scrollTop;
        const distance = targetScrollTop - currentScrollTop;

        // Om vi är nära botten, sätt scrollTop till botten och avsluta
        if (distance <= 1) {
            element.scrollTop = targetScrollTop;
            return;
        }

        // Stega neråt med scrollSpeed varje frame
        element.scrollTop += scrollSpeed;

        // Anropa nästa steg av animationen
        requestAnimationFrame(scrollStep);
    };

    // Vänta 10ms innan scrollningen börjar
    setTimeout(() => {
        targetScrollTop = element.scrollHeight - element.clientHeight;
        requestAnimationFrame(scrollStep);
    }, 50);
};
