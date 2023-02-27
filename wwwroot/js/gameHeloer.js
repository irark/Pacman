function initGame(ref){
    document.addEventListener('keydown', (event) => {
        ref.invokeMethodAsync("KeyPressed", event.key)
    }, false)
}