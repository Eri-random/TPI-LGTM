import { TestBed } from '@angular/core/testing';

import { GenerateIdeaService } from './generate-idea.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';

describe('GenerateIdeaService', () => {
  let service: GenerateIdeaService;
  let httpMock : HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[
        HttpClientTestingModule,
        ],
        providers:[
          GenerateIdeaService,
        ],
        schemas:[
            CUSTOM_ELEMENTS_SCHEMA,
            NO_ERRORS_SCHEMA
        ]
    });
    service = TestBed.inject(GenerateIdeaService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
