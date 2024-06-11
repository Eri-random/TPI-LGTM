import { TestBed } from '@angular/core/testing';

import { ResponseIdeaService } from './response-idea.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';

describe('ResponseIdeaService', () => {
  let service: ResponseIdeaService;
  let httpMock : HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
        imports:[
          HttpClientTestingModule,
      ],
      providers:[
        ResponseIdeaService
      ],
      schemas:[CUSTOM_ELEMENTS_SCHEMA,NO_ERRORS_SCHEMA]
      });
      service = TestBed.inject(ResponseIdeaService);
      httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
