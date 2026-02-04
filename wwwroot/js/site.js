function updateHeroVideo() {
    const videoElement = document.getElementById('heroVideo');
    
    if (!videoElement) return;

    const currentHour = new Date().getHours();
    let videoSrc = "";

    // 07:00 - 19:00 arası gündüz, diğer saatler gece
    if (currentHour >= 7 && currentHour < 19) {
        videoSrc = "/videos/gunduz.mp4";
    } else {
        videoSrc = "/videos/gece.mp4";
    }

    // Videonun kaynağını değiştir
    videoElement.src = videoSrc;

    // --- KRİTİK EKLEME: Videoyu yeniden yükle ve başlat ---
    videoElement.load(); 
    videoElement.play().catch(error => {
        console.log("Video otomatik başlatılamadı (Tarayıcı engeli):", error);
    });
}

// Sayfa yüklendiğinde çalıştır
document.addEventListener("DOMContentLoaded", updateHeroVideo);