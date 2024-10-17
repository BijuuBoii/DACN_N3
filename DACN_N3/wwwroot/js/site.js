// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
console.clear();

const loginBtn = document.getElementById('login');
const signupBtn = document.getElementById('signup');

loginBtn.addEventListener('click', (e) => {
	let parent = e.target.parentNode.parentNode;
	Array.from(e.target.parentNode.parentNode.classList).find((element) => {
		if (element !== "slide-up") {
			parent.classList.add('slide-up')
		} else {
			signupBtn.parentNode.classList.add('slide-up')
			parent.classList.remove('slide-up')
		}
	});
});

signupBtn.addEventListener('click', (e) => {
	let parent = e.target.parentNode;
	Array.from(e.target.parentNode.classList).find((element) => {
		if (element !== "slide-up") {
			parent.classList.add('slide-up')
		} else {
			loginBtn.parentNode.parentNode.classList.add('slide-up')
			parent.classList.remove('slide-up')
		}
	});
});



// Function to add new seasons
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

//user
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

//setting
document.querySelector('.save-btn').addEventListener('click', function () {
    alert('Settings saved successfully!');
});
