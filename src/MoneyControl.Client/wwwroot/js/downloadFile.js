window.downloadFile = (filename, content) => {
    const file = new File([content], filename);
    const exportUrl = URL.createObjectURL(file);

    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();
    a.remove();

    URL.revokeObjectURL(exportUrl);
}