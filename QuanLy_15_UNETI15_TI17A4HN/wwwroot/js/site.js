// Lumière Hotel & Resort - hiệu ứng giao diện dùng chung
(() => {
    "use strict";

    const onReady = (callback) => {
        if (document.readyState !== "loading") {
            callback();
        } else {
            document.addEventListener("DOMContentLoaded", callback);
        }
    };

    onReady(() => {
        const header = document.querySelector(".site-header");
        const progress = document.querySelector("[data-scroll-progress]");
        const backToTop = document.querySelector("[data-back-to-top]");
        const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

        const updateScrollUi = () => {
            const scrollTop = window.scrollY || document.documentElement.scrollTop;
            const scrollHeight = document.documentElement.scrollHeight - window.innerHeight;

            if (header) {
                header.classList.toggle("is-scrolled", scrollTop > 8);
            }

            if (progress) {
                const value = scrollHeight > 0 ? Math.min(100, (scrollTop / scrollHeight) * 100) : 0;
                progress.style.width = `${value}%`;
            }

            if (backToTop) {
                backToTop.classList.toggle("is-visible", scrollTop > 520);
            }
        };

        window.addEventListener("scroll", updateScrollUi, { passive: true });
        window.addEventListener("resize", updateScrollUi);
        updateScrollUi();

        if (backToTop) {
            backToTop.addEventListener("click", () => {
                window.scrollTo({
                    top: 0,
                    behavior: reduceMotion ? "auto" : "smooth"
                });
            });
        }

        const revealItems = document.querySelectorAll(".reveal");
        if (revealItems.length) {
            if (reduceMotion || !("IntersectionObserver" in window)) {
                revealItems.forEach((item) => item.classList.add("is-visible"));
            } else {
                const revealObserver = new IntersectionObserver((entries, observer) => {
                    entries.forEach((entry) => {
                        if (!entry.isIntersecting) {
                            return;
                        }

                        entry.target.classList.add("is-visible");
                        observer.unobserve(entry.target);
                    });
                }, {
                    threshold: 0.12,
                    rootMargin: "0px 0px -44px 0px"
                });

                revealItems.forEach((item) => revealObserver.observe(item));

                // Lưới an toàn: nếu trình duyệt tạm dừng IntersectionObserver,
                // nội dung vẫn hiển thị thay vì bị ẩn vĩnh viễn.
                window.setTimeout(() => {
                    if (!document.querySelector(".reveal.is-visible")) {
                        revealItems.forEach((item) => item.classList.add("is-visible"));
                    }
                }, 2200);
            }
        }

        const counters = document.querySelectorAll("[data-count]");
        const animateCounter = (element) => {
            if (element.dataset.counted === "true") {
                return;
            }

            element.dataset.counted = "true";
            const target = Number.parseInt(element.dataset.count || "0", 10);
            if (!Number.isFinite(target) || target <= 0) {
                element.textContent = "0";
                return;
            }

            if (reduceMotion) {
                element.textContent = target.toLocaleString("vi-VN");
                return;
            }

            const duration = 1100;
            const start = performance.now();
            const tick = (now) => {
                const elapsed = Math.min(1, (now - start) / duration);
                const eased = 1 - Math.pow(1 - elapsed, 3);
                element.textContent = Math.round(target * eased).toLocaleString("vi-VN");

                if (elapsed < 1) {
                    requestAnimationFrame(tick);
                } else {
                    element.textContent = target.toLocaleString("vi-VN");
                }
            };

            requestAnimationFrame(tick);
        };

        if (counters.length) {
            if (reduceMotion || !("IntersectionObserver" in window)) {
                counters.forEach((counter) => {
                    counter.textContent = counter.dataset.count || "0";
                });
            } else {
                const counterObserver = new IntersectionObserver((entries, observer) => {
                    entries.forEach((entry) => {
                        if (!entry.isIntersecting) {
                            return;
                        }

                        animateCounter(entry.target);
                        observer.unobserve(entry.target);
                    });
                }, { threshold: 0.65 });

                counters.forEach((counter) => {
                    counter.textContent = "0";
                    counterObserver.observe(counter);
                });
            }
        }
    });
})();
