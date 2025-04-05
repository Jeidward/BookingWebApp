// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
    // Function to fetch country data and populate the select element
    async function populateCountries() {
      try {
        const response = await fetch('https://restcountries.com/v3.1/all');
    const countries = await response.json();
    const select = document.getElementById('countrySelect');
        countries.sort((a, b) => a.name.common.localeCompare(b.name.common));

        countries.forEach(country => {
          const option = document.createElement('option');
    option.value = country.cca2;
    option.textContent = country.name.common;
    select.appendChild(option);
        });
      } catch (error) {
        console.error('Error fetching country data:', error);
      }
    }

document.addEventListener('DOMContentLoaded', populateCountries);




    function typeWriter3D(element, text, speed = 200) {
      const originalText = text;
    element.innerHTML = '';
    let i = 0;

    function type() {
        if (i < originalText.length) {
          const char = document.createElement('span');
    char.className = 'char';
    char.innerHTML = originalText.charAt(i);
    char.style.animationDelay = `${i * 0.1}s`;
    element.appendChild(char);
    i++;
    setTimeout(type, speed);
        }
      }

    type();
    }

    document.addEventListener('DOMContentLoaded', function() {
      const nameElement = document.getElementById('userName');
    const originalText = nameElement.innerText;
    typeWriter3D(nameElement, originalText, 200);
    });
