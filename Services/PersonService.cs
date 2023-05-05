using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts.Enums;
using System.Globalization;
using System.Net.Sockets;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            return personResponse;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //Check if "personAddRequest" is not null
            if(personAddRequest == null)
            { throw new ArgumentNullException(nameof(personAddRequest));}

            //Model Validations
            ValidationHelper.ModelValidation(personAddRequest);

            //Convert personAddRequest to person type obect
            Person person = personAddRequest.ToPerson();

            //Generate new personId
            person.PersonId = Guid.NewGuid();

            //Then add it to List<Person>
            await _personsRepository.AddPerson(person);
            
            //Return PersonResponse Object
            return ConvertPersonToPersonResponse(person);
        }

        public async Task<List<PersonResponse>> GetAllPersonsList()
        {
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(temp=>temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse> GetPersonById(Guid? personId)
        {
            //If personId is null, throw ArgumentException
            if(personId == null)
            {
                throw new ArgumentException(nameof(personId));
            }

            Person? personResponse = await _personsRepository.GetPersonByPersonId(personId);

            //If personId is not null but personId doesn't exist
            if (personResponse == null) { return null; }

            return personResponse.ToPersonResponse();
            
        }

        public async Task<List<PersonResponse>> GetFilteredList(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersonsList();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
            { return matchingPersons; }

            switch(searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase):true).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp =>
                    !string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp=>
                    (temp.DateOfBirth != null)?temp.DateOfBirth.Value.ToString("dd MMMM YYYY").Contains(searchString,StringComparison.OrdinalIgnoreCase):true).ToList();
                    break;
                case nameof(PersonResponse.ReceiveNewsLetters):
                    matchingPersons = allPersons.Where(temp =>
                    temp.ReceiveNewsLetters.ToString() == searchString).ToList();
                    break;
                default: matchingPersons = allPersons;
                    break;
            }
            return matchingPersons;
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if(string.IsNullOrEmpty(sortBy)) { return allPersons; }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName),SortOrderOptions.ASC) => allPersons.OrderBy(temp=>temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),
                _=>allPersons
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest personUpdateRequest)
        {
            if(personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            //Model Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //find the person in the existing table
            Person? personDetails = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);

            //if personDetails are null
            if(personDetails == null)
            {
                throw new ArgumentException("personId doesn't exist");
            }

            //save changes to database
            await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());

            return personDetails.ToPersonResponse();

        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            bool result = false;
            if (personId != null)
            {
                 result = await _personsRepository.DeletePersonByPersonId(personId);
            }
            return result;

        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);
            csvWriter.WriteHeader<PersonResponse>();
            csvWriter.NextRecord();

            List<PersonResponse> personResponses = (await _personsRepository.GetAllPersons()).Select(temp=>temp.ToPersonResponse()).ToList();

            await csvWriter.WriteRecordsAsync(personResponses);

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date Of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";
                worksheet.Cells["G1"].Value = "Address";
                worksheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = (await _personsRepository.GetAllPersons()).Select(temp => temp.ToPersonResponse()).ToList();

                foreach (PersonResponse person in persons)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                    {
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-mm-dd");
                    }
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
