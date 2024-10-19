document.addEventListener("DOMContentLoaded", function () {
    console.clear();

    // Xử lý sự kiện Login và Signup
    const loginBtn = document.getElementById('login');
    const signupBtn = document.getElementById('signup');

    if (loginBtn && signupBtn) {
        loginBtn.addEventListener('click', (e) => {
            let parent = e.target.parentNode.parentNode;
            Array.from(parent.classList).find((element) => {
                if (element !== "slide-up") {
                    parent.classList.add('slide-up');
                } else {
                    signupBtn.parentNode.classList.add('slide-up');
                    parent.classList.remove('slide-up');
                }
            });
        });

        signupBtn.addEventListener('click', (e) => {
            let parent = e.target.parentNode;
            Array.from(parent.classList).find((element) => {
                if (element !== "slide-up") {
                    parent.classList.add('slide-up');
                } else {
                    loginBtn.parentNode.parentNode.classList.add('slide-up');
                    parent.classList.remove('slide-up');
                }
            });
        });
    }

    //Function to add new movies
    function openAddMovieModal() {
        document.getElementById('addMovieModal').style.display = 'flex';
    }

    function closeAddMovieModal() {
        document.getElementById('addMovieModal').style.display = 'none';
    }

    function saveMovie() {
        // Code to save movie can be added here
        alert("Movie saved successfully!");
        closeAddMovieModal();
    }

    let selectedCategories = [];


    function selectCategory(element) {
        const category = element.textContent;

        // Kiểm tra nếu thể loại đã được chọn, bỏ chọn nếu đã được chọn trước
        if (selectedCategories.includes(category)) {
            selectedCategories = selectedCategories.filter(item => item !== category);
            element.classList.remove('selected');
        } else {
            // Nếu chưa chọn, thêm vào danh sách thể loại được chọn
            selectedCategories.push(category);
            element.classList.add('selected');
        }

        console.log("Selected categories:", selectedCategories);
    }

    function saveMovie() {
        // Dùng selectedCategories để lấy danh sách thể loại đã chọn
        console.log("Saving movie with categories:", selectedCategories);

        // Thêm logic lưu thông tin phim vào đây (ví dụ gửi dữ liệu lên server)

        alert("Movie saved successfully!");
        closeAddMovieModal();
    }

    // Function to add new episodes
    document.querySelectorAll('.add-episode-btn').forEach(button => {
        button.addEventListener('click', function () {
            const episodeList = this.previousElementSibling;
            const newEpisodeInput = document.createElement('input');
            newEpisodeInput.type = 'url';
            newEpisodeInput.placeholder = 'Episode Link';
            episodeList.appendChild(newEpisodeInput);
        });
    });

    // Function to add new seasons
    document.querySelectorAll('.add-season-btn').forEach(button => {
        button.addEventListener('click', function () {
            const seasonList = this.previousElementSibling;
            const seasonCount = seasonList.querySelectorAll('.season').length + 1;
            const newSeasonDiv = document.createElement('div');
            newSeasonDiv.className = 'season';
            newSeasonDiv.innerHTML = `
                <h4>Season ${seasonCount}</h4>
                <div class="episode-list">
                    <input type="url" placeholder="Episode Link">
                </div>
                <button class="add-episode-btn">Add Episode</button>
            `;
            seasonList.appendChild(newSeasonDiv);

            // Add event listener for the new episode button
            newSeasonDiv.querySelector('.add-episode-btn').addEventListener('click', function () {
                const episodeList = this.previousElementSibling;
                const newEpisodeInput = document.createElement('input');
                newEpisodeInput.type = 'url';
                newEpisodeInput.placeholder = 'Episode Link';
                episodeList.appendChild(newEpisodeInput);
            });
        });
    });

    // Xử lý nút Edit
    document.querySelectorAll('.edit-btn').forEach(button => {
        button.addEventListener('click', function () {
            alert('Edit function here!');
            // Add your edit logic here
        });
    });

    // Xử lý nút Delete
    document.querySelectorAll('.delete-btn').forEach(button => {
        button.addEventListener('click', function () {
            const row = this.closest('tr');
            row.remove(); // Xóa dòng khách hàng này
            alert('Customer deleted!');
        });
    });

    // Save settings
    const saveBtn = document.querySelector('.save-btn');
    if (saveBtn) {
        saveBtn.addEventListener('click', function () {
            alert('Settings saved successfully!');
        });
    }
});
