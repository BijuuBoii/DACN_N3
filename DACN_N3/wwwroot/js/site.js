﻿document.addEventListener("DOMContentLoaded", function () {
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
    document.querySelectorAll('.add-movie-btn').forEach(button => {
        button.addEventListener('click', function () {
            window.location.href = '/Admin/home/addMovies';
        });
    });

    document.querySelectorAll('.movie').forEach(button => {
        button.addEventListener('click', function () {
            window.location.href = '/home/movieDetails';
        });
    });

    document.querySelectorAll('.buy-ticket-button').forEach(button => {
        button.addEventListener('click', function () {
            window.location.href = '/home/selectTime';
        });
    });

    document.querySelectorAll('.time').forEach(button => {
        button.addEventListener('click', function () {
            window.location.href = `/Home/SelectChair?date=${encodeURIComponent(selectedDate)}&time=${encodeURIComponent(selectedTime)}&cinema=${encodeURIComponent(selectedCinema)}`;
        });
    });

    document.querySelectorAll('.save-btn-movies').forEach(button => {
        button.addEventListener('click', function () {
            alert("Movie saved successfully!");
            window.location.href = '/Admin/home/Movies';
        });
    });

    // Function to add new episodes
    //document.querySelectorAll('.add-season-btn').forEach(button => {
    //    button.addEventListener('click', function () {
    //        const seasonList = this.previousElementSibling;
    //        const seasonCount = seasonList.querySelectorAll('.season').length + 1;
    //        const newSeasonDiv = document.createElement('div');
    //        newSeasonDiv.className = 'season';
    //        newSeasonDiv.innerHTML = `
    //        <h4>Mùa ${seasonCount}</h4>
    //        <div class="episode-list">
    //            <ul>
    //                <!-- Tập sẽ được thêm vào đây -->
    //            </ul>
    //        </div>
    //        <button class="add-episode-btn">Thêm Tập</button>
    //        <button class="delete-season-btn">Xóa Mùa</button>
    //    `;
    //        seasonList.appendChild(newSeasonDiv);

    //        // Thêm sự kiện cho nút thêm tập
    //        newSeasonDiv.querySelector('.add-episode-btn').addEventListener('click', function () {
    //            const episodeList = this.previousElementSibling.querySelector('ul');
    //            const episodeCount = episodeList.querySelectorAll('.episode').length + 1; // Đếm số tập hiện tại
    //            const newEpisodeItem = document.createElement('li');
    //            newEpisodeItem.className = 'episode';
    //            newEpisodeItem.innerHTML = `
    //            Tập ${episodeCount}: <input type="url" placeholder="Episode Link">
    //            <button class="delete-episode-btn">Xóa Tập</button>
    //        `;
    //            episodeList.appendChild(newEpisodeItem);
    //        });
    //    });
    //});

    //// Xóa tập phim hoặc mùa phim
    //document.addEventListener('click', function (e) {
    //    // Xóa mùa phim
    //    if (e.target.classList.contains('delete-season-btn')) {
    //        e.target.closest('.season').remove();
    //    }

    //    // Xóa tập phim
    //    if (e.target.classList.contains('delete-episode-btn')) {
    //        e.target.closest('li').remove();
    //    }
    //});
    

    const heartButton = document.getElementById('heartButton');
    const heartIcon = heartButton.querySelector('i'); // Lấy biểu tượng trái tim bên trong

    // Thêm sự kiện click để toggle lớp 'active' và đổi từ regular sang solid
    heartButton.addEventListener('click', function () {
        heartButton.classList.toggle('active');

        // Toggle giữa các lớp 'fa-regular' và 'fa-solid' cho biểu tượng
        heartIcon.classList.toggle('fa-regular');
        heartIcon.classList.toggle('fa-solid');
    });


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

    


    


    

    //Function to close Add Movie Modal
    function closeAddMovieModal() {
        document.getElementById('addMovieModal').style.display = 'none';
    }

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

    document.querySelectorAll('.seat').forEach(seat => {
        seat.addEventListener('click', function () {
            if (seat.classList.contains("placed")) return;

            // Kiểm tra nếu số ghế đã chọn ít hơn 8 hoặc nếu đã chọn ghế này thì có thể bỏ chọn
            if (selectedSeats.length < 8 || seat.classList.contains('selected')) {
                toggleSeatSelection(seat);
            } else {
                showPopup();  // Hiển thị thông báo khi quá 8 ghế
            }
            var totalPrice = document.getElementById("total-price").innerText;
            totalPrice = totalPrice.replace('.', '').replace('đ', '').trim();
            totalPrice = totalPrice + '000'; // Thêm 3 số 0 vào cuối

            // Gán giá trị vào trường "Amount" trong form Momo
            document.getElementById("momoAmount").value = totalPrice;
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



/*function deleteSeason(button) {
    button.closest('.season').remove();
}

function deleteEpisode(button) {
    button.closest('li').remove();
}*/

function addCategory() {
    let newCategory = prompt("Nhập tên thể loại mới:");
    if (newCategory) {
        let categoryList = document.querySelector('.category-list');
        let newDiv = document.createElement('div');
        newDiv.classList.add('category');
        newDiv.setAttribute('onclick', 'selectCategory(this)');
        newDiv.innerHTML = newCategory + ' <button onclick="deleteCategory(this)">Xóa</button>';
        categoryList.appendChild(newDiv);
    }
}

function deleteCategory(button) {
    button.parentElement.remove();
}

//Genres
function addGenre() {
    let genreName = prompt("Nhập tên thể loại mới:");
    if (genreName) {
        let genreList = document.getElementById('genreList');
        let newItem = document.createElement('li');
        newItem.innerHTML = genreName + ' <button class="delete-btn" onclick="deleteGenre(this)">Xóa</button>';
        genreList.appendChild(newItem);
    }
}

function deleteGenre(button) {
    button.parentElement.remove();
}

function showSuggestions() {
    document.getElementById('suggestions').style.display = 'block';
}

// Lọc gợi ý theo từ khóa
function filterSuggestions() {
    const input = document.getElementById('genreInput').value.toLowerCase();
    const suggestions = document.querySelectorAll('#suggestions li');

    suggestions.forEach(function (suggestion) {
        if (suggestion.textContent.toLowerCase().includes(input)) {
            suggestion.style.display = 'block';
        } else {
            suggestion.style.display = 'none';
        }
    });
}

// Khi chọn thể loại
function selectGenre(genre) {
    const selectedGenresDiv = document.getElementById('selectedGenres');

    // Kiểm tra nếu thể loại đã được chọn
    if (!document.getElementById(genre)) {
        const genreTag = document.createElement('span');
        genreTag.className = 'genre-tag';
        genreTag.id = genre;
        genreTag.innerHTML = `${genre} <button class="remove-btn" onclick="removeGenre('${genre}')">X</button>`;
        selectedGenresDiv.appendChild(genreTag);
    }

    // Xóa input và ẩn danh sách gợi ý
    document.getElementById('genreInput').value = '';
    document.getElementById('suggestions').style.display = 'none';
}

// Xóa thể loại khi nhấn vào nút X
function removeGenre(genre) {
    document.getElementById(genre).remove();
}

function addNewGenre() {
    const genreInput = document.getElementById('genreInput');
    const newGenre = genreInput.value.trim();

    if (newGenre !== "") {
        // Thêm thể loại vào danh sách các thể loại đã chọn
        selectGenre(newGenre);

        // Thêm thể loại mới vào danh sách gợi ý
        const suggestionList = document.getElementById('suggestions');
        const newSuggestion = document.createElement('li');
        newSuggestion.textContent = newGenre;
        newSuggestion.onclick = function () {
            selectGenre(newGenre);
        };
        suggestionList.appendChild(newSuggestion);

        // Thêm thể loại mới vào danh sách các thể loại gợi ý trong input
        const option = document.createElement('option');
        option.value = newGenre;
        option.textContent = newGenre;
        document.getElementById('genreSelect').appendChild(option);

        // Xóa nội dung trong input
        genreInput.value = '';
    } else {
        alert("Vui lòng nhập tên thể loại!");
    }
}

// Ẩn danh sách gợi ý khi click ra ngoài
document.addEventListener('click', function (event) {
    const suggestions = document.getElementById('suggestions');
    const genreInput = document.getElementById('genreInput');

    if (!suggestions.contains(event.target) && !genreInput.contains(event.target)) {
        suggestions.style.display = 'none';
    }
});

document.addEventListener('click', function (event) {
    const suggestions = document.getElementById('suggestions');
    const genreInput = document.getElementById('genreInput');

    if (!suggestions.contains(event.target) && !genreInput.contains(event.target)) {
        suggestions.style.display = 'none';
    }
});

document.querySelector('form').addEventListener('submit', function () {
    const selectedSeatsList = JSON.parse(sessionStorage.getItem('selectedSeats'));
    document.getElementById('selected-seats-input').value = selectedSeatsList.join(',');
});



let currentIndex = 0;
const slides = document.querySelectorAll('.carousel-slide');

function showSlide(index) {
    slides.forEach((slide, i) => {
        slide.style.transform = `translateX(${100 * (i - index)}%)`;
    });
}

function nextSlide() {
    currentIndex = (currentIndex + 1) % slides.length;
    showSlide(currentIndex);
}

setInterval(nextSlide, 1000); // Tự động chuyển slide mỗi 3 giây

//selectChair
const seatsContainer = document.getElementById('seats');
const selectedSeatsElement = document.getElementById('selected-seats');
const totalPriceElement = document.getElementById('total-price');

const SEAT_COUNT = 60;
const VIP_SEATS = [14,15, 16,17,24, 25, 26,27,34,37,44,47, 35, 36,45,46]; // Các ghế VIP
const REGULAR_PRICE = 45;
const VIP_PRICE = 60;
const condition = true;

let selectedSeats = [];
let totalPrice = 0;
let temp = 0;
let selectedDate = '19/11/2024';  // Biến lưu trữ ngày chọn
let selectedTime = '';  // Biến lưu trữ giờ chọn

/*let selectedSeats = [];*/

function selectChair(seat) {
    if (seat.classList.contains("placed")) return;

    // Kiểm tra nếu số ghế đã chọn ít hơn 8 hoặc nếu đã chọn ghế này thì có thể bỏ chọn
    if (selectedSeats.length < 8 || seat.classList.contains('selected')) {
        toggleSeatSelection(seat);
    } else {
        showPopup();  // Hiển thị thông báo khi quá 8 ghế
    }

    sessionStorage.setItem('selectedSeats', JSON.stringify(selectedSeats));

    var totalPrice = document.getElementById("total-price").innerText;

    // Làm sạch giá trị tiền, ví dụ: 1000000 -> 1000000
    totalPrice = totalPrice.replace('.', '').replace('đ', '').trim();

    // Thêm 3 số 0 vào cuối (ví dụ: 1000 -> 1000000)
    totalPrice = totalPrice + '000'; // Thêm 3 số 0 vào cuối

    // Gán giá trị vào trường "Amount" trong form Momo
    document.getElementById("momoAmount").value = totalPrice;
}
// Chọn hoặc bỏ chọn ghế
function toggleSeatSelection(seat) {
    const seatNumber = seat.textContent;
    const seatPrice = parseInt(seat.dataset.price);

    if (seat.classList.contains('selected')) {
        // Bỏ chọn ghế
        seat.classList.remove('selected');
        selectedSeats = selectedSeats.filter(s => s !== seatNumber);
        totalPrice -= seatPrice;
    } else {
        // Chọn ghế
        seat.classList.add('selected');
        selectedSeats.push(seatNumber);
        totalPrice += seatPrice;
    }

    updateSeatInfo();
}
// Cập nhật thông tin ghế và tổng tiền
function updateSeatInfo() {
    selectedSeatsElement.textContent = selectedSeats.join(', ') || 'Chưa chọn';
    totalPriceElement.textContent = totalPrice;
}

function showPopup() {
    document.getElementById('popup').style.display = 'block';
    document.getElementById('overlay').style.display = 'block';
}

function closePopup() {
    document.getElementById('popup').style.display = 'none';
    document.getElementById('overlay').style.display = 'none';
}

function toggleCinemaDetails(header) {
    const details = header.nextElementSibling;
    details.classList.toggle('hidden');
}

function selectDate(element) {
    // Xóa lớp 'selected' khỏi tất cả các ngày
    const dates = document.querySelectorAll('.date-item');
    dates.forEach(date => date.classList.remove('active'));

    // Thêm lớp 'selected' vào ngày được chọn
    element.classList.add('active');
    selectedDate = element.innerText.trim();  // Lấy ngày đã chọn
    console.log('Selected Date: ', selectedDate);
}

function selectTime(element) {
    if (!element.classList.contains('disabled')) {
        selectedTime = element.innerText.split('\n')[0].trim();  // Lấy giờ đã chọn
        selectedCinema = element.querySelector('.Rap').innerText.trim();
        
        console.log('Selected Time: ', selectedTime);
        console.log('Selected Cinema: ', selectedCinema);
    }
}

function updateSelectedSeats() {
    // Lấy giá trị ghế đã chọn từ #selected-seats
    var selectedSeats = document.getElementById("selected-seats").innerText;

    // Gán giá trị ghế đã chọn vào input hidden #selected-seats-input
    document.getElementById("selected-seats-input").value = selectedSeats;
}
