let lastScroll = 0;

window.addEventListener("scroll", () => {
    const current = window.scrollY;

    // collapse when scrolling down more than ~10px
    if (current > lastScroll + 10) {
        try {
            DotNet.invokeMethodAsync("DavPadDev.Client", "CollapseFromScroll");
        } catch { }
    }

    lastScroll = current;
});
